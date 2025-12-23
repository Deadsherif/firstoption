# Manual Rebuild Guide: AuthService Add-in

This guide provides a step-by-step outline to rebuild the `AuthService` add-in manually. Follow this order to handle dependencies correctly (building from the bottom up).

## Phase 1: Project Setup & Configuration

1.  **Create Project**
    *   **Type**: Class Library (.NET Framework)
    *   **Framework**: .NET Framework 4.8 (or 4.8.1)
    *   **Name**: `AuthService`

2.  **Add References (NuGet)**
    *   `System.Text.Json` (Latest version compatible with .NET 4.8)
    *   `RevitAPI` & `RevitAPIUI` (Add references to the Revit DLLs usually found in `C:\Program Files\Autodesk\Revit 20xx\`)
    *   `PresentationCore`, `PresentationFramework`, `WindowsBase`, `System.Xaml` (Add framework assemblies for WPF)

---

## Phase 2: Data Models & Helpers (The "Model" Layer)

**File: `Services/DateTimeConverter.cs`**
*   **Purpose**: Handles JSON parsing for dates (ISO 8601 & SQL formats).
*   **Class**: `DateTimeConverter : JsonConverter<DateTime>`
*   **Methods**:
    *   `Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)` -> Returns `DateTime`.
    *   `Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)` -> Returns `void`.

**File: `Services/ApiResponseGet.cs`**
*   **Purpose**: Strongly typed classes for the API JSON response.
*   **Classes**:
    *   `ApiResponseGet`: (Properties: `Code` (int), `Success` (bool), `Message` (string), `Payload` (PayloadData))
    *   `PayloadData`: (Properties: `Subscription` (Subscription), `Redemptions` (List<Redemption>))
    *   `Subscription`: (Properties: `StartDate` (DateTime), `EndDate` (DateTime), `IsTrial` (bool), `Status` (string), etc.)
        *   *Note: Apply `[JsonConverter(typeof(DateTimeConverter))]` to date properties.*
    *   `Redemption`: (Properties: `Id` (int), `IpAddress` (string), etc.)

---

## Phase 3: Services (The "Business Logic" Layer)

**File: `Services/DateHelper.cs`**
*   **Purpose**: Gets trusted current date from the web with fallbacks.
*   **Class**: `DateHelper` (static)
*   **Methods**:
    *   `GetCurrentDateFromWebAsync()` -> Returns `Task<DateTime>`
        *   *Logic*: Set TLS 1.2. Try `aisenseapi.com`, catch error, try `ipgeolocation.io`, catch error, fallback to `DateTime.MinValue`, ultimate fallback to `DateTime.UtcNow`.
*   **Private Classes**: `AisenseApiResponse`, `IpGeolocationApiResponse` (Internal models for the specific date APIs).

**File: `Services/SubscriptionService.cs`**
*   **Purpose**: Handles communication with your backend.
*   **Class**: `SubscriptionService`
*   **Properties**:
    *   `Status` -> Type: `SubscriptionStatus` (Enum: Valid, Expired, NotFound, Error)
*   **Methods**:
    *   `Constructor()`: Initialize `HttpClient`, set Headers (`User-Agent`, `Accept`).
    *   `GetRedemptionsAsync(string code)` -> Returns `Task<ApiResponseGet>`
        *   *Logic*: Encode code, build URL, call API, check success code, deserialize JSON. Return `null` on timeout/exception.
    *   `Dispose()` -> Returns `void`.

---

## Phase 4: MVVM Core

**File: `MVVM/ViewModel/ViewModelBase.cs`**
*   **Purpose**: Boilerplate for property change notifications.
*   **Class**: `ViewModelBase : INotifyPropertyChanged`
*   **Methods**:
    *   `OnPropertyChanged([CallerMemberName] string propertyName)` -> `void`
    *   `SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName)` -> `bool`

**File: `Commands/RelayCommand.cs`**
*   **Purpose**: Boilerplate for binding buttons to functions.
*   **Class**: `RelayCommand : ICommand`
*   **Constructor**: Takes `Action<object> execute` and optional `Func<object, bool> canExecute`.
*   **Methods**: `Execute`, `CanExecute`, `RaiseCanExecuteChanged`.

---

## Phase 5: ViewModels (The "Application Logic" Layer)

**File: `MVVM/ViewModel/AuthViewModel.cs`**
*   **Purpose**: Controls the UI state and calls services.
*   **Class**: `AuthViewModel : ViewModelBase`
*   **Properties**:
    *   `Code` (string), `ServerResponse` (string), `Status` (string), `StatusForeground` (Brush), `IsLoading` (bool).
    *   `SubmitCommand` (ICommand), `CloseCommand` (ICommand).
*   **Methods**:
    *   `Constructor()`: Init service, commands, set default colors.
    *   `ExecuteSubmit(object parameter)` -> `async void`
        *   *Logic*: Show loading, call `GetRedemptionsAsync`, validate response, check date expiry using `DateHelper`, update UI status (Red/Green).
    *   `ExecuteClose(object parameter)` -> `void` (Closes window).
    *   `CanExecuteClose(object parameter)` -> `bool` (Only true if Status == Valid).

---

## Phase 6: Views (The "UI" Layer)

**File: `MVVM/View/AuthWindow.xaml`**
*   **Purpose**: The actual window layout.
*   **Key Elements**:
    *   `Window`: Title="Authentication".
    *   `Resources`: Add `BooleanToVisibilityConverter`.
    *   `Grid`: 5 Rows.
    *   `TextBox`: Bind to `Code`.
    *   `Button` ("Check"): Bind to `SubmitCommand`.
    *   `Button` ("OK"): Bind to `CloseCommand`.
    *   `TextBlock` (Response): Bind to `ServerResponse`, Visibility bound to `IsLoading`.
    *   `TextBlock` (Status): Bind to `Status` and `StatusForeground`.

**File: `MVVM/View/AuthWindow.xaml.cs`**
*   **Purpose**: Code-behind.
*   **Constructor**:
    *   `InitializeComponent()`
    *   `DataContext = new AuthViewModel()`

---

## Phase 7: Revit Integration

**File: `Command.cs`**
*   **Purpose**: Entry point for Revit.
*   **Attributes**: `[Transaction(TransactionMode.Manual)]`
*   **Class**: `Command : IExternalCommand`
*   **Methods**:
    *   `Execute(ExternalCommandData, ref string, ElementSet)` -> Returns `Result`
        *   *Logic*: Instantiate `AuthWindow`, call `.ShowDialog()`, return `Result.Succeeded`.

**File: `AuthService.addin`**
*   **Purpose**: Register tool with Revit.
*   **Content**: XML defining the Assembly path, ClientId, and FullClassName (`AuthService.Command`).
