# 🚗 CarsXE API (.NET Package)

[![NuGet version](https://img.shields.io/nuget/v/CarsXE.svg?cacheSeconds=0)](https://www.nuget.org/packages/CarsXE)

**CarsXE** is a powerful and developer-friendly API that gives you instant access to a wide range of vehicle data. From VIN decoding and market value estimation to vehicle history, images, OBD code explanations, and plate recognition, CarsXE provides everything you need to build automotive applications at scale.

🌐 **Website:** [https://api.carsxe.com](https://api.carsxe.com)  
📄 **Docs:** [https://api.carsxe.com/docs](https://api.carsxe.com/docs)  
📦 **All Products:** [https://api.carsxe.com/all-products](https://api.carsxe.com/all-products)

To get started with the CarsXE API, follow these steps:

1. **Sign up for a CarsXE account:**
   - [Register here](https://api.carsxe.com/register)
   - Add a [payment method](https://api.carsxe.com/dashboard/billing#payment-methods) to activate your subscription and get your API key.

2. **Install the CarsXE NuGet package:**

   Run this command in your terminal:

   ```bash
   dotnet add package CarsXE
   ```

3. **Import the CarsXE API into your code:**

   ```csharp
   using carsxe;
   ```

4. **Initialize the API with your API key:**

   ```csharp
   string API_KEY = "YOUR_API_KEY";
   CarsXE carsxe = new CarsXE(API_KEY);
   ```

5. **Use the various endpoint methods provided by the API to access the data you need.**

## Usage

```csharp
string vin = "WBAFR7C57CC811956";

try
{
    var specs = carsxe.Specs(new Dictionary<string, string> { { "vin", vin } }).Result;
    Console.WriteLine("API Response:");
    Console.WriteLine(specs.RootElement.GetProperty("input").GetProperty("vin").ToString());
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

---

## 📚 Endpoints

The CarsXE API provides the following endpoint methods:

### `Specs` – Decode VIN & get full vehicle specifications

**Required:**

- `vin`

**Optional:**

- `deepdata`
- `disableIntVINDecoding`

**Example:**

```csharp
var vehicle = carsxe.Specs(new Dictionary<string, string> { { "vin", "WBAFR7C57CC811956" } }).Result;
```

---

### `InternationalVinDecoder` – Decode VIN with worldwide support

**Required:**

- `vin`

**Optional:**

- None

**Example:**

```csharp
var intvin = carsxe.InternationalVinDecoder(new Dictionary<string, string> { { "vin", "WF0MXXGBWM8R43240" } }).Result;
```

---

### `PlateDecoder` – Decode license plate info (plate, country)

**Required:**

- `plate`
- `country` (always required except for US, where it is optional and defaults to 'US')

**Optional:**

- `state` (required for some countries, e.g. US, AU, CA)
- `district` (required for Pakistan)

> **Note:**
>
> - The `state` parameter is required only when applicable (for
>   specific countries such as US, AU, CA, etc.).
> - For Pakistan (`country='pk'`), both `state` and `district`
>   are required.

**Example:**

```csharp
var decodedPlate = carsxe.PlateDecoder(new Dictionary<string, string>
{
    { "plate", "7XER187" },
    { "state", "CA" },
    { "country", "US" }
}).Result;
```

---

### `UsPlateDecoder` – Decode a US license plate (plate, state)

**Required:**

- `plate`
- `state`

**Optional:**

- `decodeVIN`

**Example:**

```csharp
var usPlate = carsxe.UsPlateDecoder(new Dictionary<string, string>
{
    { "plate", "H37SFS" },
    { "state", "NJ" },
    { "decodeVIN", "true" }
}).Result;
```

---

### `MarketValue` – Estimate vehicle market value based on VIN

**Required:**

- `vin`

**Optional:**

- `state`
- `mileage`
- `condition`

**Example:**

```csharp
var marketvalue = carsxe.MarketValue(new Dictionary<string, string>
{
    { "vin", "WBAFR7C57CC811956" },
    { "mileage", "50000" },
    { "condition", "clean" },
    { "state", "CA" }
}).Result;
```

---

### `History` – Retrieve vehicle history

**Required:**

- `vin`

**Optional:**

- None

**Example:**

```csharp
var history = carsxe.History(new Dictionary<string, string> { { "vin", "WBAFR7C57CC811956" } }).Result;
```

---

### `Images` – Fetch images by make, model, year, trim

**Required:**

- `make`
- `model`

**Optional:**

- `year`
- `trim`
- `color`
- `transparent`
- `angle`
- `photoType`
- `size`
- `license`

**Example:**

```csharp
var images = carsxe.Images(new Dictionary<string, string>
{
    { "make", "BMW" },
    { "model", "X5" },
    { "year", "2019" }
}).Result;
```

---

### `Recalls` – Get safety recall data for a VIN

**Required:**

- `vin`

**Optional:**

- None

**Example:**

```csharp
var recalls = carsxe.Recalls(new Dictionary<string, string> { { "vin", "1C4JJXR64PW696340" } }).Result;
```

---

### `RecallsYmm` – Get safety recall data by year, make, and model

**Required:**

- `year`
- `make`
- `model`

**Optional:**

- None

**Example:**

```csharp
var recallsYmm = carsxe.RecallsYmm(new Dictionary<string, string>
{
    { "year", "2026" },
    { "make", "toyota" },
    { "model", "corolla" }
}).Result;
```

---

### `RecallsBatchSubmit` – Submit up to 10,000 VINs for bulk recall checking

**Required:**

- at least one of `vins`, `csv`, or `csvUrl` in the request body

**Optional:**

- `webhookUrl`

**Example:**

```csharp
var batch = carsxe.RecallsBatchSubmit(new
{
    vins = new[] { "1HGBH41JXMN109186", "5YJSA1E26HF000001", "1C4JJXR64PW696340" },
    webhookUrl = "https://your-server.com/webhook"
}).Result;
```

---

### `RecallsBatchStatus` – Check the status of a recall batch

**Required:**

- `batchId`

**Optional:**

- None

**Example:**

```csharp
var status = carsxe.RecallsBatchStatus(new Dictionary<string, string> { { "batchId", "brb_mnablbn7_wvbaqv" } }).Result;
```

---

### `RecallsBatchResults` – Retrieve JSON results for a completed recall batch

**Required:**

- `batchId`

**Optional:**

- None

**Example:**

```csharp
var results = carsxe.RecallsBatchResults(new Dictionary<string, string> { { "batchId", "brb_mnablbn7_wvbaqv" } }).Result;
```

---

### `RecallsBatchDownload` – Download recall batch results as CSV

**Required:**

- `batchId`

**Optional:**

- None

> **Note:** This method returns CSV text (`string`), not a `JsonDocument`.

**Example:**

```csharp
var csv = carsxe.RecallsBatchDownload(new Dictionary<string, string> { { "batchId", "brb_mnablbn7_wvbaqv" } }).Result;
```

---

### `PlateImageRecognition` – Read & decode plates from images

**Required:**

- `imageUrl`

**Optional:**

- None

**Example:**

```csharp
var plateimg = carsxe.PlateImageRecognition("https://api.carsxe.com/img/apis/plate_recognition.JPG").Result;
```

---

### `VinOcr` – Extract VINs from images using OCR

**Required:**

- `imageUrl`

**Optional:**

- None

**Example:**

```csharp
var vinocr = carsxe.VinOcr("https://api.carsxe.com/img/apis/plate_recognition.JPG").Result;
```

---

### `YearMakeModel` – Query vehicle by year, make, model and trim (optional)

**Required:**

- `year`
- `make`
- `model`

**Optional:**

- `trim`

**Example:**

```csharp
var yymm = carsxe.YearMakeModel(new Dictionary<string, string>
{
    { "year", "2012" },
    { "make", "BMW" },
    { "model", "5 Series" }
}).Result;
```

---

### `YmmOptions` – List years, makes, models, variants, or trims for dropdowns

**Required:**

- None

**Optional:**

- `dimension` (`years` | `makes` | `models` | `trims` | `variants`)
- `year`
- `make`
- `model`
- `trim`

**Example:**

```csharp
var years = carsxe.YmmOptions().Result;
var makes = carsxe.YmmOptions(new Dictionary<string, string> { { "year", "2026" } }).Result;
var models = carsxe.YmmOptions(new Dictionary<string, string> { { "make", "Toyota" } }).Result;
var variants = carsxe.YmmOptions(new Dictionary<string, string>
{
    { "year", "2026" },
    { "make", "Toyota" },
    { "model", "Tacoma" }
}).Result;
```

---

### `ObdCodesDecoder` – Decode OBD error/diagnostic codes

**Required:**

- `code`

**Optional:**

- None

**Example:**

```csharp
var obdcode = carsxe.ObdCodesDecoder(new Dictionary<string, string> { { "code", "P0115" } }).Result;
```

---

### `LienTheft` – Check active liens and theft records by VIN

**Required:**

- `vin`

**Optional:**

- None

**Example:**

```csharp
var lienAndTheft = carsxe.LienAndTheft(new Dictionary<string, string> { { "vin", "2C3CDXFG1FH762860" } }).Result;
```

---

### `OwnershipVin` – Look up registered owner(s) by VIN

**Required:**

- `vin`

**Optional:**

- `include` (comma-separated subset of `demographics,emails,phones,vehicle_history`)

**Example:**

```csharp
var owners = carsxe.OwnershipVin(new Dictionary<string, string> { { "vin", "1FT8X3BT0BEA61538" } }).Result;
```

---

### `OwnershipPerson` – Resolve contact details by name and address

**Required:**

- `first_name`
- `last_name`
- `address`
- `zip`

**Optional:**

- `include`

**Example:**

```csharp
var person = carsxe.OwnershipPerson(new Dictionary<string, string>
{
    { "first_name", "John" },
    { "last_name", "Sample" },
    { "address", "123 Example St" },
    { "zip", "90210" }
}).Result;
```

---

### `OwnershipAddress` – Find residents at a street address

**Required:**

- `address`
- `zip`

**Optional:**

- `include`
- `variant` (legacy alias)

**Example:**

```csharp
var residents = carsxe.OwnershipAddress(new Dictionary<string, string>
{
    { "address", "123 Example St" },
    { "zip", "90210" }
}).Result;
```

---

### `OwnershipZip` – Search people in a ZIP code with optional filters

**Required:**

- `zip`

**Optional:**

- `gender`
- `min_age`
- `max_age`
- `income`
- `page`
- `limit`
- `include`
- `variant` (legacy alias)

**Example:**

```csharp
var zipSearch = carsxe.OwnershipZip(new Dictionary<string, string>
{
    { "zip", "00000" },
    { "gender", "f" },
    { "min_age", "45" }
}).Result;
```

---

## Async/Await Usage (Recommended)

For better performance and non-blocking operations, use async/await instead of .Result:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using carsxe;

class Program
{
    static async Task Main(string[] args)
    {
        string API_KEY = "YOUR_API_KEY_HERE";
        await using var carsxe = new CarsXE(API_KEY);

        try
        {
            var specs = await carsxe.Specs(new Dictionary<string, string> { { "vin", "WBAFR7C57CC811956" } });
            Console.WriteLine("Year: " + specs.RootElement.GetProperty("attributes").GetProperty("year").ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

## Notes & Best Practices

- **Parameter requirements:** Each endpoint requires specific parameters—see the Required/Optional fields above.
- **Return values:** Responses are JsonDocument objects for easy access and manipulation using System.Text.Json. `RecallsBatchDownload` is the exception and returns CSV text as a `string`.
- **Error handling:** Use try/catch blocks to gracefully handle API errors.
- **Async operations:** Use async/await for better performance instead of blocking with .Result.
- **Resource management:** The CarsXE client implements IAsyncDisposable, so use `await using` or call DisposeAsync() when done.
- **More info:** For advanced usage and full details, visit the [official API documentation](https://api.carsxe.com/docs).

---

## Overall

CarsXE API provides a wide range of powerful, easy-to-use tools for accessing and integrating vehicle data into your .NET applications and services. Whether you're a developer or a business owner, you can quickly get the information you need to take your projects to the next level—without hassle or inconvenience.
