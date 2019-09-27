
function GetHash {
    param(
        [Parameter(Mandatory)]
        [String]$ResourcePath
    )

    $md5 = New-Object -TypeName System.Security.Cryptography.MD5CryptoServiceProvider
    [System.BitConverter]::ToString($md5.ComputeHash([System.IO.File]::ReadAllBytes($ResourcePath)))
}

function ResolveGeneratedPath {
    param(
        [Parameter(Mandatory)]
        [String]$ResourcePath,
        [Parameter(Mandatory)]
        [String]$GeneratedFileName
    )

    $resource = Get-Item $ResourcePath
    Join-Path $resource.Directory.FullName $GeneratedFileName
}

function Test-Resource {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [String]$ResourcePath,
        [Parameter(Mandatory)]
        [String]$GeneratedFileName
    )


    $hash = GetHash $ResourcePath
    $writePath = ResolveGeneratedPath $ResourcePath $GeneratedFileName

    $match = Select-String -Path $writePath -Pattern "^// Hash: $hash$"

    if(!($match)){
        Write-Error "$writePath does not have a matching hash for $ResourcePath - Regenerate the resource class"
    }
}

function Update-Resource {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [String]$ResourcePath,
        [Parameter(Mandatory)]
        [String]$GeneratedFileName,
        [Parameter(Mandatory)]
        [String]$Namespace,
        [Parameter(Mandatory)]
        [String]$ClassName,
        [Parameter(Mandatory)]
        [String]$AccessModifier
    )

    $commandName = $PSCmdlet.MyInvocation.MyCommand
    $hash = GetHash $ResourcePath
    $writePath = ResolveGeneratedPath $ResourcePath $GeneratedFileName
    $resources = ([xml](Get-Content -Path (Get-Item $ResourcePath).FullName -Raw)).root.data

    function WriteEntry {
        param(
            [Parameter(Mandatory, ValueFromPipeline)]
            [PSObject[]]$Data
        )

        process{
            foreach($entry in $Data) {
                $spacer = "        "
                $commentEntry = "$spacer/// <summary>$([Environment]::NewLine)" +
                                "$spacer///   Looks up a localized string similar to: $($entry.value)$([Environment]::NewLine)" +
                                "$spacer/// </summary>$([Environment]::NewLine)"

                $codeEntry = if($entry.comment) {
                    $params = ($entry.comment -split ',') | ForEach-Object { "object $($_.Trim())" }
                    $paramString = $params -join ', '
                    "$spacer$AccessModifier static string $($entry.name)($paramString) => string.Format(Culture, GetString(nameof($($entry.name))), $($entry.comment));$([Environment]::NewLine)"
                } else {
                    "$spacer$AccessModifier static string $($entry.name) => GetString(nameof($($entry.name)));$([Environment]::NewLine)"
                }

                $commentEntry + $codeEntry
            }
        }
    }

$entries = ($resources | WriteEntry) -join [Environment]::NewLine

$content = @"
// Hash: $hash
// Auto generated - Do not modify this file!
// Modify $($Resource.Name) then invoke $commandName to recreate
// For members that require formatting provide a csv of the argument names
// $(Get-Date)
namespace $Namespace {

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("$commandName", "0.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    $AccessModifier class $ClassName {

        private static global::System.Resources.ResourceManager resourceMan;

        /// <summary>
        ///   Initializes a new instance of the <see cref="$ClassName"/> class.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        $AccessModifier $ClassName() {}

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        $AccessModifier static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("$Namespace.$ClassName", typeof($ClassName).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        $AccessModifier static global::System.Globalization.CultureInfo Culture { get; set; }

        $entries
        /// <summary>
        ///    Returns the resource string for the given key
        /// </summary>
        private static string GetString(string key) => ResourceManager.GetString(key, Culture);
    }
}
"@

    Set-Content -Path $writePath -Value $content
    Write-Output "Generated $writePath"
}

Export-ModuleMember -Function *-*
