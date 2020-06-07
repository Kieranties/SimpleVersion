Describe "Get-Cactus" {
    It "Returns 🌵" {
        "🌵" | Should -Be '🌵'
    }
}

Describe "Calling tool" {
    It "Returns" {
        simpleversion | Should -BeLike '*Error*'
    }
}