# Files/Folders to Delete Before Deployment

## 1. Build Artifacts (Auto-generated)
- /.angular/          # 18 MB - Build cache
- /dist/              # 1.7 MB - Build output

## 2. Documentation Files (Not needed for deployment)
- API_IMPLEMENTATION_SUMMARY.md
- BACKEND_COUNTRY_STATE_API.md
- BUILD_FIXES.md
- COMPREHENSIVE_PROJECT_REVIEW.md
- CREATEINVOICE_TEST_FIXES.md
- CRITICAL_MEDIUM_FIXES_APPLIED.md
- EDIT_INVOICE_FIELD_BINDING_FIX.md
- EDIT_INVOICE_UNAUTHORIZED_FIX.md
- FINAL_INVOICE_REVIEW.md
- INVOICE_COMPLETE_FIX.md
- INVOICE_EDIT_FIX.md
- INVOICE_FIELD_BINDING_FIX.md
- INVOICE_FIXES_SUMMARY.md
- INVOICE_NUMBER_FIX.md
- PDF_GENERATION_FIXES.md
- PDF_ISSUE_ANALYSIS.md
- QUICK_START_TEST_INVOICES.md
- TEST_INVOICE_GENERATOR_INSTRUCTIONS.md
- UI_TEST_EXECUTION_GUIDE.md
- UI_TEST_INSTRUCTIONS.md
- UI_TEST_QUICK_REFERENCE.md

## 3. SQL Scripts (Keep in separate folder, not in Angular project)
- CREATE_COUNTRY_STATE_TABLES.sql
- FIX_PROD_UNIQUEKEY.sql
- RESET_CUSTOMER_TABLE.sql

## 4. Test Components (If not using)
- src/app/Component/test-invoice-generator/
- src/app/Component/ui-test-runner/
- src/app/_service/create-invoice-ui-test.service.ts
- src/app/_service/test-data-generator.service.ts

## 5. Git folder (If pushing to new repo)
- /.git/              # ~16 MB - Git history

## Commands to Clean:

```bash
# Delete build artifacts
rmdir /s /q .angular
rmdir /s /q dist

# Delete documentation (optional)
del *.md

# Delete SQL scripts (move to database folder first)
del *.sql

# To start fresh with Git (optional)
rmdir /s /q .git
git init
```

## What to Keep:
- node_modules (needed for development, excluded in .gitignore)
- src/ (your source code)
- package.json, package-lock.json
- angular.json, tsconfig.json
- .gitignore
- vercel.json

## For Deployment:
- Vercel/Netlify will automatically exclude node_modules
- They will run `npm install` on their servers
- Only push source code to Git
