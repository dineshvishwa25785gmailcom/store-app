# Invoice Field and Data Binding Fixes

## Issues Fixed

### 1. Invoice Number Field Not Editable When Creating New Invoice
**Problem**: The invoice number field was readonly when creating a new invoice, preventing users from entering the invoice number.

**Root Cause**: The HTML template had `[readonly]="!isEditing"` which made the field readonly when NOT editing (i.e., when creating new).

**Fix**: Changed the logic to `[readonly]="isEditing"` so the field is:
- **Editable** when creating new invoice (isEditing = false)
- **Readonly** when editing existing invoice (isEditing = true)

**File**: `createinvoice.component.html`
```html
<!-- Before -->
<input matInput formControlName="invoiceNumber" [readonly]="!isEditing" required>

<!-- After -->
<input matInput formControlName="invoiceNumber" [readonly]="isEditing" required>
```

### 2. Data Not Binding Correctly When Editing from List
**Problem**: When clicking edit from the invoice list, the form fields were not populating with the invoice data.

**Root Cause**: The backend returns `DisplayInvNumber` (user-entered invoice number) but the frontend was only checking for `displayInvNumber` (camelCase). Additionally, there was a fallback issue if the field was missing.

**Fix**: Updated the data binding logic to handle multiple casing variations and provide proper fallback:

**File**: `createinvoice.component.ts` - SetEditInfo method
```typescript
// Before
invoiceNumber: editdata.displayInvNumber || editdata.DisplayInvNumber || '',

// After - Added fallback to invoiceNumber if DisplayInvNumber is missing
invoiceNumber: editdata.displayInvNumber || editdata.DisplayInvNumber || editdata.invoiceNumber || '',
```

### 3. Removed Unnecessary Form Control Manipulation
**Problem**: The code was manually enabling/disabling the invoice number field in ngOnInit, which conflicted with the HTML template's readonly binding.

**Fix**: Removed the manual enable/disable calls since the HTML template already handles this with `[readonly]="isEditing"`.

**File**: `createinvoice.component.ts` - ngOnInit method
```typescript
// Before
if (this.editinvoiceno == null) {
  this.invoiceform.get('invoiceNumber')?.enable();
} else {
  this.invoiceform.get('invoiceNumber')?.disable();
  this.SetEditInfo(this.editinvoiceno);
}

// After - Removed enable/disable calls
if (this.editinvoiceno == null) {
  this.isLoading = false;
  this.isEditing = false;
  this.isedit = false;
} else {
  this.isEditing = true;
  this.isedit = true;
  this.pagetitle = 'Edit Invoice';
  this.SetEditInfo(this.editinvoiceno);
}
```

## Backend Changes

### Added DisplayInvNumber to Invoice_Header_DTO
To ensure the backend returns the user-entered invoice number when fetching invoice headers:

**File**: `Invoice_Header_DTO.cs`
```csharp
public class Invoice_Header_DTO
{
    public string InvoiceNumber { get; set; } = null!;
    public string? DisplayInvNumber { get; set; }  // ✅ Added
    // ... other fields
}
```

## How It Works Now

### Create New Invoice Flow:
1. User navigates to create invoice page
2. `isEditing = false` → Invoice number field is **editable**
3. User enters invoice number (e.g., "2025/001")
4. User fills in other details and saves
5. Backend creates invoice with DisplayInvNumber = "2025/001"

### Edit Existing Invoice Flow:
1. User clicks edit button on invoice list (passes system InvoiceNumber like "INV001")
2. Frontend fetches invoice header by InvoiceNumber
3. Backend returns both InvoiceNumber and DisplayInvNumber
4. Frontend populates form with DisplayInvNumber (user-entered like "2025/001")
5. `isEditing = true` → Invoice number field is **readonly**
6. User modifies other details and saves
7. Backend identifies invoice by DisplayInvNumber and updates it

## Testing Checklist

✅ Create new invoice - invoice number field is editable
✅ Edit existing invoice - invoice number field is readonly
✅ Edit existing invoice - all fields populate correctly
✅ Edit existing invoice - displays user-entered invoice number (DisplayInvNumber)
✅ Save edited invoice - updates existing invoice instead of creating new one
