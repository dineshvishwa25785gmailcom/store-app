import { Routes } from '@angular/router';
import { HomeComponent } from './Component/home/home.component';
import { RegisterComponent } from './Component/register/register.component';
import { LoginComponent } from './Component/login/login.component';
import { ConfirmotpComponent } from './Component/confirmotp/confirmotp.component';
import { ForgetpasswordComponent } from './Component/forgetpassword/forgetpassword.component';
import { UpdatepasswordComponent } from './Component/updatepassword/updatepassword.component';
import { ResetpasswordComponent } from './Component/resetpassword/resetpassword.component';
import { CustomerComponent } from './Component/customer/customer.component';
import { UserComponent } from './Component/user/user.component';
import { authGuard } from './_guard/auth.guard';
import { AddcustomerComponent } from './Component/addcustomer/addcustomer.component';
import { UserroleComponent } from './Component/userrole/userrole.component';
import { CreateinvoiceComponent } from './Component/createinvoice/createinvoice.component';
import { ListinvoiceComponent } from './Component/listinvoice/listinvoice.component';
import { RatingComponent } from './Component/rating/rating.component';
import { TestInvoiceGeneratorComponent } from './Component/test-invoice-generator/test-invoice-generator.component';
import { UITestRunnerComponent } from './Component/ui-test-runner/ui-test-runner.component';
import { CategoryComponent } from './Component/category/category.component';
import { ProductComponent } from './Component/product/product.component';
 
export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [authGuard],
  },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  { path: 'login', component: LoginComponent },
  { path: 'confirmotp', component: ConfirmotpComponent },
  { path: 'forgetpassword', component: ForgetpasswordComponent },
  { path: 'updatepassword', component: UpdatepasswordComponent },
  { path: 'resetpassword', component: ResetpasswordComponent },
  { path: 'customer', component: CustomerComponent, canActivate: [authGuard] },
  {
    path: 'customer/add',
    component: AddcustomerComponent,
    canActivate: [authGuard],
  },
  {
    path: 'customer/edit/:uniqueKeyID',
    component: AddcustomerComponent,
    canActivate: [authGuard],
  },
  { path: 'user', component: UserComponent, canActivate: [authGuard] },
  { path: 'userrole', component: UserroleComponent, canActivate: [authGuard] },
  { path: 'createinvoice', component: CreateinvoiceComponent, canActivate: [authGuard] },
  { path: 'editinvoice/:invoiceno', component: CreateinvoiceComponent, canActivate: [authGuard] },
  { path: 'listinvoice', component: ListinvoiceComponent, canActivate: [authGuard] },
  { path: 'rating', component: RatingComponent, canActivate: [authGuard] },
  { path: 'category', component: CategoryComponent, canActivate: [authGuard] },
  { path: 'productcategory', component: CategoryComponent, canActivate: [authGuard] },
  { path: 'product', component: ProductComponent, canActivate: [authGuard] },
  { path: 'productdetails', component: ProductComponent, canActivate: [authGuard] },
  { path: 'test-invoices', component: TestInvoiceGeneratorComponent, canActivate: [authGuard] },
  { path: 'ui-test', component: UITestRunnerComponent, canActivate: [authGuard] },
];
