// src/GitVisualizer/Services/AppTheme.cs
using MudBlazor;

namespace GitVisualizer.Services;

public static class AppTheme
{
    public static readonly MudTheme Dark = new MudTheme
    {
        PaletteDark = new PaletteDark
        {
            Primary             = "#3FB950",   // GitHub green (dark)
            PrimaryContrastText = "#FFFFFF",
            Secondary           = "#58A6FF",   // GitHub blue (dark)
            Error               = "#F0B429",   // Amber — NEVER red for errors
            Warning             = "#E3B341",
            Success             = "#3FB950",
            Background          = "#0D1117",   // GitHub dark background
            Surface             = "#161B22",   // GitHub dark surface
            DrawerBackground    = "#161B22",
            AppbarBackground    = "#161B22",
            AppbarText          = "#E6EDF3",
            TextPrimary         = "#E6EDF3",
            TextSecondary       = "#8B949E",
            ActionDefault       = "#8B949E",
            Divider             = "#30363D",
            DividerLight        = "#21262D",
            OverlayDark         = "rgba(1,4,9,0.7)",
        }
    };

    public static readonly MudTheme Light = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary             = "#2DA44E",   // GitHub green (light)
            PrimaryContrastText = "#FFFFFF",
            Secondary           = "#0969DA",   // GitHub blue (light)
            Error               = "#E6A817",   // Amber (light)
            Warning             = "#BF8700",
            Success             = "#2DA44E",
            Background          = "#FFFFFF",
            Surface             = "#F6F8FA",
            DrawerBackground    = "#F6F8FA",
            AppbarBackground    = "#24292F",   // Dark header even in light mode (GitHub pattern)
            AppbarText          = "#FFFFFF",
            TextPrimary         = "#24292F",
            TextSecondary       = "#57606A",
            ActionDefault       = "#57606A",
            Divider             = "#D0D7DE",
            DividerLight        = "#EAEEF2",
            OverlayDark         = "rgba(27,31,36,0.5)",
        }
    };
}
