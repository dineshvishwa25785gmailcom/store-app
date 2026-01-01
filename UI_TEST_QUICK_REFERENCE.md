# UI Test - Quick Reference Card

## 🚀 QUICK START (3 Steps)

### 1. Start & Login
```bash
ng serve
# Open: http://localhost:4200
# Login with credentials
```

### 2. Run Tests
```
Navigate to: http://localhost:4200/ui-test
Click: "Run Tests" button
Wait: 5-10 seconds
```

### 3. Verify Results
```
✅ Check: Success rate = 100%
✅ View: Invoice list for UI-TEST-001, UI-TEST-002
✅ Query: Database for inserted records
```

---

## 📊 WHAT GETS TESTED

| # | Test | Database Impact |
|---|------|-----------------|
| 1 | Load Customers | None (read) |
| 2 | Load Products | None (read) |
| 3 | Create Invoice 1 | INSERT 1 invoice + 1 item |
| 4 | Create Invoice 2 | INSERT 1 invoice + 2 items |
| 5-7 | Validations | None (frontend) |
| 8-11 | Calculations | None (math) |
| 12-13 | Edit Invoice | None (read) |

**Total Database Inserts**: 2 invoices + 3 items

---

## 🔍 VERIFY DATABASE

```sql
-- Quick check
SELECT display_inv_number, total_amount 
FROM tbl_Invoice 
WHERE display_inv_number LIKE 'UI-TEST%';

-- Expected:
-- UI-TEST-001 | 500.00
-- UI-TEST-002 | 1750.00
```

---

## ✅ SUCCESS CRITERIA

- [ ] 13/13 tests passed
- [ ] 100% success rate
- [ ] 2 invoices in list
- [ ] 2 records in tbl_Invoice
- [ ] 3 records in tbl_sales_productinfo

---

## 🐛 QUICK FIXES

**No customers?**
→ Add customer at `/customer/add`

**No products?**
→ Add products via backend

**Tests fail?**
→ Check browser console (F12)
→ Check backend is running on port 86

---

## 🧹 CLEANUP

```sql
DELETE FROM tbl_sales_productinfo 
WHERE invoice_number IN (
  SELECT invoice_number FROM tbl_Invoice 
  WHERE display_inv_number LIKE 'UI-TEST%'
);

DELETE FROM tbl_Invoice 
WHERE display_inv_number LIKE 'UI-TEST%';
```

---

## 📞 SUPPORT

- Full Guide: `UI_TEST_EXECUTION_GUIDE.md`
- Test Code: `create-invoice-ui-test.service.ts`
- UI Component: `ui-test-runner.component.ts`
