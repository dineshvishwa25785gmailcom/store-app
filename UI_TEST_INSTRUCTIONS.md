# CreateInvoice UI Test Suite - Instructions

## Overview
Automated UI testing for the CreateInvoice page with sample data to ensure functionality is working correctly.

## Files Created
1. `create-invoice-ui-test.service.ts` - Test execution service
2. `ui-test-runner.component.ts` - Visual test runner UI
3. Route: `/ui-test`

## Test Coverage

### 1. Load Page Data
- ✅ Load customers from API
- ✅ Load products from API
- ✅ Verify data is available

### 2. Create Single Product Invoice
- ✅ Create invoice with 1 product
- ✅ Quantity: 5 units
- ✅ Verify API response
- ✅ Verify invoice number generated

### 3. Create Multiple Product Invoice
- ✅ Create invoice with 2 products
- ✅ Quantities: 10 + 5 units
- ✅ Verify total calculation
- ✅ Verify API response

### 4. Validation Tests
- ✅ Empty invoice number validation
- ✅ Zero quantity validation
- ✅ No products validation

### 5. Calculation Tests
- ✅ Test: 5 × 100 = 500
- ✅ Test: 10 × 150 = 1500
- ✅ Test: 3 × 99.99 = 299.97
- ✅ Total sum calculation
- ✅ Decimal rounding (3 decimals for items, 2 for total)

### 6. Edit Invoice
- ✅ Load invoice header
- ✅ Load invoice details
- ✅ Verify data binding

## How to Use

### Method 1: Using UI Test Runner (Recommended)

1. **Start the application**:
   ```bash
   ng serve
   ```

2. **Login to the application**

3. **Navigate to test runner**:
   ```
   http://localhost:4200/ui-test
   ```

4. **Click "Run Tests" button**

5. **View results**:
   - Total tests executed
   - Passed/Failed count
   - Success rate percentage
   - Detailed results for each test

### Method 2: Using Browser Console

1. **Open application and login**

2. **Open Developer Console (F12)**

3. **Run tests programmatically**:
   ```javascript
   // Get Angular injector
   const injector = ng.probe(document.querySelector('app-root')).injector;
   
   // Get test service
   const testService = injector.get('CreateInvoiceUITestService');
   
   // Run tests
   testService.runCompleteTest();
   ```

4. **Check console for detailed output**

## Test Results Format

### Console Output
```
🧪 Starting CreateInvoice UI Test Suite...

📋 Test 1: Loading Page Data...
✅ Load Customers: ✅ Loaded 5 customers
✅ Load Products: ✅ Loaded 10 products

📋 Test 2: Create Single Product Invoice...
✅ Create Single Product Invoice: ✅ Invoice created: INV001

📋 Test 3: Create Multiple Product Invoice...
✅ Create Multiple Product Invoice: ✅ Invoice created: INV002

📋 Test 4: Testing Validation...
✅ Validation: Empty Invoice Number: ✅ Correctly rejected
✅ Validation: Zero Quantity: ✅ Frontend validates
✅ Validation: No Products: ✅ Frontend validates

📋 Test 5: Testing Calculations...
✅ Calculation 1: 5 × 100: ✅ Result: 500
✅ Calculation 2: 10 × 150: ✅ Result: 1500
✅ Calculation 3: 3 × 99.99: ✅ Result: 299.97
✅ Total Calculation: ✅ Total: 2299.97

📋 Test 6: Testing Edit Invoice...
✅ Edit Invoice: Load Header: ✅ Loaded: UI-TEST-001
✅ Edit Invoice: Load Details: ✅ Loaded 1 items

============================================================
📊 TEST SUMMARY
============================================================
Total Tests: 15
✅ Passed: 15
❌ Failed: 0
Success Rate: 100.00%
============================================================
```

### UI Display
- **Summary Cards**: Total, Passed, Failed, Success Rate
- **Test List**: Each test with pass/fail icon
- **Details**: Test name, status, message, timestamp

## Sample Test Data

### Invoice 1: UI-TEST-001
```json
{
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

### Invoice 2: UI-TEST-002
```json
{
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

## Prerequisites

Before running tests:
- ✅ Backend API running on `http://localhost:86`
- ✅ At least 1 customer in database
- ✅ At least 1 product in database
- ✅ User logged in to application

## Verification

After tests complete:

1. **Check Console Output**:
   - All tests should show ✅
   - Success rate should be 100%

2. **Verify Created Invoices**:
   ```
   Navigate to: http://localhost:4200/listinvoice
   Look for: UI-TEST-001, UI-TEST-002
   ```

3. **Test Edit Functionality**:
   - Click edit on UI-TEST-001
   - Verify all fields populate
   - Verify products load correctly

## Troubleshooting

### Error: "No customers found"
**Solution**: Create at least one customer
```
Navigate to: http://localhost:4200/customer/add
Add customer details and save
```

### Error: "No products found"
**Solution**: Add products via backend or product management

### Error: "Failed to create invoice"
**Check**:
1. Backend API is running
2. Database connection is active
3. Customer/Product IDs are valid
4. Browser console for detailed errors

### Error: "Test execution failed"
**Check**:
1. Network connectivity
2. API endpoints are accessible
3. CORS is configured correctly
4. Authentication token is valid

## Expected Results

### All Tests Pass (100%)
```
Total Tests: 15
✅ Passed: 15
❌ Failed: 0
Success Rate: 100.00%
```

### Some Tests Fail
If tests fail, check:
- Backend API responses
- Database data integrity
- Network connectivity
- Browser console errors

## Cleanup

To remove test invoices:

1. **Via UI**:
   - Navigate to invoice list
   - Delete invoices starting with "UI-TEST"

2. **Via SQL**:
   ```sql
   DELETE FROM tbl_sales_productinfo 
   WHERE invoice_number IN (
     SELECT invoice_number FROM tbl_Invoice 
     WHERE display_inv_number LIKE 'UI-TEST%'
   );
   
   DELETE FROM tbl_Invoice 
   WHERE display_inv_number LIKE 'UI-TEST%';
   ```

## Test Maintenance

### Adding New Tests
Edit `create-invoice-ui-test.service.ts`:

```typescript
private async testNewFeature() {
  console.log('\n📋 Test X: Testing New Feature...');
  
  try {
    // Test logic here
    const success = true; // Your test condition
    
    this.logTest('New Feature Test', success, 
      success ? '✅ Test passed' : '❌ Test failed');
  } catch (error: any) {
    this.logTest('New Feature Test', false, `❌ Error: ${error.message}`);
  }
}
```

Add to `runCompleteTest()`:
```typescript
await this.testNewFeature();
```

### Modifying Test Data
Update payload in `createInvoicePayload()` method to change:
- Invoice numbers
- Quantities
- Products
- Customer data

## Performance

### Test Execution Time
- **Average**: 5-10 seconds
- **Depends on**:
  - Network speed
  - Backend response time
  - Number of tests

### Optimization Tips
- Run tests during off-peak hours
- Ensure good network connectivity
- Keep backend database optimized

## Security Notes

⚠️ **Important**:
- Test data uses prefix "UI-TEST" for easy identification
- Tests create real data in database
- Clean up test data after testing
- Do not run in production environment

## Integration with CI/CD

To integrate with automated testing:

```bash
# Run tests via CLI
ng test

# Or use Playwright/Cypress for E2E
npx playwright test
```

## Support

For issues or questions:
1. Check browser console for errors
2. Review test results in UI
3. Check backend API logs
4. Verify database state
5. Review this documentation

---

**Build Status**: ✅ SUCCESS
**Ready to Use**: YES
**Last Updated**: 2025
