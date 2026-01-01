# UI Test Execution Guide - CreateInvoice Page

## 🚀 STEP-BY-STEP EXECUTION

### Prerequisites Check
Before running tests, verify:
```bash
# 1. Check backend is running
curl http://localhost:86/api/Customer/GetAll

# 2. Check database has data
# - At least 1 customer
# - At least 1 product

# 3. Start Angular app
ng serve
```

---

## 📋 TEST EXECUTION STEPS

### Step 1: Login to Application
1. Open browser: `http://localhost:4200`
2. Navigate to login page
3. Enter credentials and login
4. Verify you're authenticated

### Step 2: Navigate to Test Runner
1. Go to: `http://localhost:4200/ui-test`
2. You should see "CreateInvoice UI Test Suite" page
3. Verify test coverage list is displayed

### Step 3: Run Tests
1. Click **"Run Tests"** button
2. Wait for "Running tests... Please wait" message
3. Tests will execute automatically (5-10 seconds)
4. Results will display on screen

### Step 4: Review Results
Expected output format:
```
Results: 13/13 Passed (100%)

✅ Load Customers: ✅ Loaded X customers
✅ Load Products: ✅ Loaded X products
✅ Create Single Product Invoice: ✅ Invoice created: INV001
✅ Create Multiple Product Invoice: ✅ Invoice created: INV002
✅ Validation: Empty Invoice Number: ✅ Correctly rejected
✅ Validation: Zero Quantity: ✅ Frontend validates
✅ Validation: No Products: ✅ Frontend validates
✅ Calculation 1: 5 × 100: ✅ Result: 500
✅ Calculation 2: 10 × 150: ✅ Result: 1500
✅ Calculation 3: 3 × 99.99: ✅ Result: 299.97
✅ Total Calculation: ✅ Total: 2299.97
✅ Edit Invoice: Load Header: ✅ Loaded: UI-TEST-001
✅ Edit Invoice: Load Details: ✅ Loaded 1 items
```

### Step 5: Verify Database Inserts
1. Click **"View Invoices"** button
2. Look for invoices with numbers: `UI-TEST-001`, `UI-TEST-002`
3. Verify invoice details:

**Expected Invoice 1 (UI-TEST-001)**:
- Customer: First customer in database
- Products: 1 item
- Quantity: 5
- Total: 500 (or 5 × product rate)

**Expected Invoice 2 (UI-TEST-002)**:
- Customer: First customer in database
- Products: 2 items
- Quantities: 10 + 5
- Total: 1750 (or calculated based on product rates)

### Step 6: Verify Database Records
Open SQL Server Management Studio or query tool:

```sql
-- Check created invoices
SELECT 
    display_inv_number,
    invoice_number,
    customer_id,
    total_amount,
    create_date
FROM tbl_Invoice
WHERE display_inv_number LIKE 'UI-TEST%'
ORDER BY create_date DESC;

-- Expected Result:
-- UI-TEST-001 | INV001 | CUST001 | 500.00 | 2025-01-XX
-- UI-TEST-002 | INV002 | CUST001 | 1750.00 | 2025-01-XX

-- Check invoice items
SELECT 
    i.display_inv_number,
    s.product_id,
    s.quantity,
    s.rate_with_tax,
    s.amount
FROM tbl_Invoice i
JOIN tbl_sales_productinfo s ON i.invoice_number = s.invoice_number
WHERE i.display_inv_number LIKE 'UI-TEST%'
ORDER BY i.display_inv_number, s.product_id;

-- Expected Result for UI-TEST-001:
-- UI-TEST-001 | PROD001 | 5 | 100.00 | 500.00

-- Expected Result for UI-TEST-002:
-- UI-TEST-002 | PROD001 | 10 | 100.00 | 1000.00
-- UI-TEST-002 | PROD002 | 5 | 150.00 | 750.00
```

---

## 🧪 DETAILED TEST SCENARIOS

### Test 1: Load Page Data
**What it tests**: API endpoints for customers and products

**Expected Behavior**:
- GET `/api/Customer/GetAll` returns array of customers
- GET `/api/Product/GetAll` returns array of products
- Both arrays have at least 1 item

**Database Impact**: None (read-only)

**Success Criteria**:
- ✅ Customers loaded: Count > 0
- ✅ Products loaded: Count > 0

---

### Test 2: Create Single Product Invoice
**What it tests**: Invoice creation with 1 product

**API Call**:
```
POST /api/Invoice/Save
Body: {
  "displayInvNumber": "UI-TEST-001",
  "customerId": "CUST001",
  "products": [
    {
      "productId": "PROD001",
      "quantity": 5,
      "rateWithTax": 100,
      "amount": 500
    }
  ],
  "totalAmount": 500
}
```

**Database Impact**:
- INSERT into `tbl_Invoice` (1 row)
- INSERT into `tbl_sales_productinfo` (1 row)

**Success Criteria**:
- ✅ Response: `{ "result": "pass", "kyValue": "INV001" }`
- ✅ Invoice created in database
- ✅ Invoice items created in database

**Verify in Database**:
```sql
SELECT * FROM tbl_Invoice WHERE display_inv_number = 'UI-TEST-001';
SELECT * FROM tbl_sales_productinfo WHERE invoice_number = 'INV001';
```

---

### Test 3: Create Multiple Product Invoice
**What it tests**: Invoice creation with 2 products

**API Call**:
```
POST /api/Invoice/Save
Body: {
  "displayInvNumber": "UI-TEST-002",
  "customerId": "CUST001",
  "products": [
    {
      "productId": "PROD001",
      "quantity": 10,
      "rateWithTax": 100,
      "amount": 1000
    },
    {
      "productId": "PROD002",
      "quantity": 5,
      "rateWithTax": 150,
      "amount": 750
    }
  ],
  "totalAmount": 1750
}
```

**Database Impact**:
- INSERT into `tbl_Invoice` (1 row)
- INSERT into `tbl_sales_productinfo` (2 rows)

**Success Criteria**:
- ✅ Response: `{ "result": "pass", "kyValue": "INV002" }`
- ✅ Invoice created with correct total
- ✅ Both products inserted

**Verify in Database**:
```sql
SELECT * FROM tbl_Invoice WHERE display_inv_number = 'UI-TEST-002';
SELECT * FROM tbl_sales_productinfo WHERE invoice_number = 'INV002';
-- Should return 2 rows
```

---

### Test 4: Validation Tests
**What it tests**: Frontend and backend validation

**Test 4a: Empty Invoice Number**
- Attempt to create invoice with empty `displayInvNumber`
- Expected: Validation error or rejection
- Database Impact: None (validation fails)

**Test 4b: Zero Quantity**
- Frontend validates quantity > 0
- Expected: Warning message before API call
- Database Impact: None (prevented by frontend)

**Test 4c: No Products**
- Frontend validates at least 1 product
- Expected: Warning message before API call
- Database Impact: None (prevented by frontend)

**Success Criteria**:
- ✅ All validation checks pass
- ✅ No invalid data reaches database

---

### Test 5: Calculation Tests
**What it tests**: Mathematical calculations

**Test Cases**:
1. 5 × 100 = 500 ✅
2. 10 × 150 = 1500 ✅
3. 3 × 99.99 = 299.97 ✅
4. Total: 500 + 1500 + 299.97 = 2299.97 ✅

**Database Impact**: None (calculation only)

**Success Criteria**:
- ✅ All calculations correct
- ✅ Proper decimal rounding (3 decimals for items, 2 for total)

---

### Test 6: Edit Invoice
**What it tests**: Loading invoice for editing

**API Calls**:
```
GET /api/Invoice/InvoiceHeaderController?invoiceno=INV001
GET /api/Invoice/InvoiceSalesItemController?invoiceno=INV001
```

**Database Impact**: None (read-only)

**Success Criteria**:
- ✅ Header loads with correct data
- ✅ Items load with correct quantities and rates
- ✅ Data matches what was inserted

**Verify**:
```sql
-- Check header
SELECT * FROM tbl_Invoice WHERE invoice_number = 'INV001';

-- Check items
SELECT * FROM tbl_sales_productinfo WHERE invoice_number = 'INV001';
```

---

## 📊 EXPECTED DATABASE STATE AFTER TESTS

### tbl_Invoice Table
```
| invoice_number | display_inv_number | customer_id | total_amount | create_by   |
|----------------|-------------------|-------------|--------------|-------------|
| INV001         | UI-TEST-001       | CUST001     | 500.00       | UITestUser  |
| INV002         | UI-TEST-002       | CUST001     | 1750.00      | UITestUser  |
```

### tbl_sales_productinfo Table
```
| invoice_number | product_id | quantity | rate_with_tax | amount  |
|----------------|------------|----------|---------------|---------|
| INV001         | PROD001    | 5.000    | 100.000       | 500.000 |
| INV002         | PROD001    | 10.000   | 100.000       | 1000.000|
| INV002         | PROD002    | 5.000    | 150.000       | 750.000 |
```

---

## ✅ SUCCESS CHECKLIST

After running tests, verify:

- [ ] All 13 tests show ✅ (green checkmark)
- [ ] Success rate is 100%
- [ ] No error messages displayed
- [ ] 2 invoices visible in invoice list (UI-TEST-001, UI-TEST-002)
- [ ] Database has 2 new invoice records
- [ ] Database has 3 new invoice item records
- [ ] Invoice totals are calculated correctly
- [ ] Can edit UI-TEST-001 invoice successfully
- [ ] Can view PDF for created invoices
- [ ] Can delete test invoices

---

## 🐛 TROUBLESHOOTING

### Issue: "No customers found"
**Solution**:
```sql
-- Add test customer
INSERT INTO tbl_customer (unique_key_id, name, email, phone, is_active)
VALUES ('CUST001', 'Test Customer', 'test@example.com', '1234567890', 1);
```

### Issue: "No products found"
**Solution**:
```sql
-- Add test products
INSERT INTO tbl_product (unique_key_id, product_name, rate_with_tax)
VALUES 
('PROD001', 'Test Product 1', 100.00),
('PROD002', 'Test Product 2', 150.00);
```

### Issue: "Failed to create invoice"
**Check**:
1. Backend API logs for errors
2. Database connection
3. Foreign key constraints
4. Required fields in database

### Issue: Tests show ❌ (red X)
**Debug Steps**:
1. Open browser console (F12)
2. Check Network tab for failed API calls
3. Review error messages in console
4. Check backend API logs
5. Verify database state

---

## 🧹 CLEANUP

After testing, remove test data:

```sql
-- Delete test invoice items
DELETE FROM tbl_sales_productinfo 
WHERE invoice_number IN (
    SELECT invoice_number FROM tbl_Invoice 
    WHERE display_inv_number LIKE 'UI-TEST%'
);

-- Delete test invoices
DELETE FROM tbl_Invoice 
WHERE display_inv_number LIKE 'UI-TEST%';

-- Verify cleanup
SELECT COUNT(*) FROM tbl_Invoice WHERE display_inv_number LIKE 'UI-TEST%';
-- Should return 0
```

Or use the UI:
1. Go to invoice list
2. Delete UI-TEST-001
3. Delete UI-TEST-002

---

## 📸 SCREENSHOT CHECKLIST

Capture these screenshots for documentation:

1. ✅ Test runner page before execution
2. ✅ Test runner showing "Running tests..."
3. ✅ Test results showing 100% success
4. ✅ Invoice list showing UI-TEST invoices
5. ✅ Database query results showing inserted records
6. ✅ Edit invoice page with loaded data

---

## 🎯 NEXT STEPS

After successful test execution:

1. **Document Results**: Save screenshots and test output
2. **Verify Functionality**: Manually test create invoice page
3. **Performance Test**: Create 10+ invoices to test performance
4. **Edge Cases**: Test with special characters, large quantities
5. **Integration Test**: Test full workflow from create to PDF generation

---

**Test Execution Time**: 5-10 seconds
**Database Records Created**: 2 invoices, 3 invoice items
**API Calls Made**: ~10 calls
**Success Rate Expected**: 100%
