# Invoice Edit Creating New Invoice - Fix

## Problem
When editing an existing invoice, the system was creating a new invoice instead of updating the existing one.

## Root Cause
The backend uses `DisplayInvNumber` (user-entered invoice number like "2025/001") to identify which invoice to update. However, the frontend was loading the system-generated `InvoiceNumber` (like "INV001") into the form's `invoiceNumber` field when editing, causing the backend to not find the existing invoice and create a new one instead.

## Backend Logic (InvoiceContainer.cs - SaveHeader method)
```csharp
var existingHeader = await _DBContext.TblInvoicesHeaders
    .FirstOrDefaultAsync(item => item.DisplayInvNumber == invoiceHeader.DisplayInvNumber);

if (existingHeader != null)
{
    // UPDATE existing invoice
    existingHeader.InvoiceNumber = generatedInvoiceNumber; // Generates NEW InvoiceNumber
    // ... update other fields
}
else
{
    // CREATE new invoice
    var newHeader = mapper.Map<InvoiceCreateDTO, TblInvoiceHeader>(invoiceHeader);
    newHeader.InvoiceNumber = generatedInvoiceNumber;
    await _DBContext.TblInvoicesHeaders.AddAsync(newHeader);
}
```

## Two Invoice Number System
1. **DisplayInvNumber**: User-entered invoice number (e.g., "2025/001") - used for display and identifying invoices for updates
2. **InvoiceNumber**: System-generated unique key (e.g., "INV001") - used for internal operations and PDF generation

## Changes Made

### 1. Backend - Invoice_Header_DTO.cs
Added `DisplayInvNumber` field to the DTO so it's returned when fetching invoice headers:

```csharp
public class Invoice_Header_DTO
{
    public string InvoiceNumber { get; set; } = null!;
    public string? DisplayInvNumber { get; set; }  // ✅ Added
    // ... other fields
}
```

### 2. Frontend - createinvoice.component.ts
Updated `SetEditInfo` method to load `DisplayInvNumber` into the form instead of `InvoiceNumber`:

**Before:**
```typescript
this.invoiceform.patchValue({
    invoiceNumber: editdata.invoiceNumber || '',  // ❌ Wrong - system-generated number
    // ...
});
```

**After:**
```typescript
this.invoiceform.patchValue({
    invoiceNumber: editdata.displayInvNumber || editdata.DisplayInvNumber || '',  // ✅ Correct - user-entered number
    // ...
});
```

## How It Works Now

### Create Flow:
1. User enters invoice number "2025/001" in form
2. Frontend sends `displayInvNumber: "2025/001"`
3. Backend checks if "2025/001" exists → NOT FOUND
4. Backend creates new invoice with:
   - `DisplayInvNumber = "2025/001"`
   - `InvoiceNumber = "INV001"` (auto-generated)

### Edit Flow:
1. User clicks edit on invoice with `DisplayInvNumber = "2025/001"` and `InvoiceNumber = "INV001"`
2. Frontend loads invoice header and populates form with `DisplayInvNumber = "2025/001"`
3. User modifies invoice and saves
4. Frontend sends `displayInvNumber: "2025/001"`
5. Backend checks if "2025/001" exists → FOUND
6. Backend updates existing invoice (generates new InvoiceNumber for tracking)

## Result
✅ Editing an invoice now correctly updates the existing invoice instead of creating a new one
✅ The user-entered invoice number (DisplayInvNumber) is preserved
✅ The system maintains proper invoice tracking with auto-generated InvoiceNumbers
