// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


namespace Trifon.IDP.Pages.Ciba;

public static class ConsentOptions
{
    public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";
    public static readonly string InvalidSelectionErrorMessage = "Invalid selection";

    public static bool EnableOfflineAccess { get; } = true;

    public static string OfflineAccessDisplayName { get; } = "Offline Access";

    public static string OfflineAccessDescription { get; } = "Access to your applications and resources, even when you are offline";

}
