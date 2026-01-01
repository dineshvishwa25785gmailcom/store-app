import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  loginresp,
  menu,
  menupermission,
  menus,
  registerconfirm,
  resetpassword,
  roles,
  updatepassword,
  UpdateRole,
  Updatestatus,
  usercred,
  userregister,
  users,
  TblRolepermission,
} from '../_model/user.model';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = environment.apiUrl; // 'http://localhost:86/api/User';
  _registerresp = signal<registerconfirm>({
    userid: 0,
    username: '',
    otptext: '',
  });

  constructor(private http: HttpClient) {}
  username = signal('');
  _menulist = signal<menu[]>([]);

  Userregisteration(data: userregister): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post(`${this.baseUrl}User/userregistration`, data, {
      headers,
    });
  }

  Confirmregisteration(data: registerconfirm): Observable<any> {
    console.log('Payload:', data); // Debugging log
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post(`${this.baseUrl}User/confirmregisteration`, data, {
      headers,
    });
  }

  //Confirmregisteration(_data: registerconfirm): Observable<any> {
   // const headers = new HttpHeaders({
    ////  'Content-Type': 'application/json',
    //});
    // Construct the URL with query parameters
   // const url = `${this.baseUrl}User/confirmregisteration?userid=${_data.userid}&username=${_data.username}&otptext=${_data.otptext}`;
   // console.log('URL sent to API:', url);
    // Send the POST request with no body (null)
    //return this.http.post(url, null, { headers });
  //}

  // Proceed with login and return the response as an observable
  // The response type is specified as 'loginresp'  to match the expected structure
  // The method takes 'usercred' as input, which contains the username and password for login
  // The method uses HttpClient to send a POST request to the API endpoint for generating a token
  // The headers are set to 'application/json' to indicate the content type of the request
  // The method returns an observable of type 'loginresp' which can be subscribed to in the component
  // to handle the response
  Proceedlogin(_data: usercred): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post<loginresp>(
      `${this.baseUrl}Authorize/GenerateToken`,
      _data,
      { headers }
    );
  }

  GenerateRefreshToken(tokenData: any): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post(
      `${this.baseUrl}Authorize/GenerateRefreshToken`,
      tokenData,
      { headers }
    );
  }

  Loadmenubyrole(role: string) {
    return this.http.get<menu[]>(
      this.baseUrl + 'UserRole/GetAllMenusByRole?userrole=' + role
    );
  }
  Resetpassword(_data: resetpassword): Observable<any> {
    console.log('Payload:', _data); // Debugging log
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    return this.http.post(`${this.baseUrl}User/resetpassword`, _data, {
      headers,
    });
  }

  Forgetpassword(username: string) : Observable<any> {
     const headers = new HttpHeaders({
       'Content-Type': 'application/json',
     });
     // Construct the URL with query parameters
    const url = `${this.baseUrl}User/forgotpassword?username=${username}`;
   console.log('URL sent to API:', url);
    // Send the POST request with no body (null)
    return this.http.post(url, null, { headers });
   // return this.http.get(this.baseUrl + 'User/forgotpassword?username=' + username)
  }

  Updatepassword(_data: updatepassword): Observable<any> {
    console.log('Payload:', _data); // Debugging log
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    return this.http.post(`${this.baseUrl}User/updatepassword`, _data, {
      headers,
    });
  }


  Getmenupermission(role:string,menuname:string){
    return this.http.get<menupermission>(this.baseUrl + 'UserRole/GetMenusPermissionByRole?userrole='+role+'&menucode=' + menuname)
  }




  Getallusers() {
    return this.http.get<users[]>(this.baseUrl + 'User/GetAll');
  }

  GetUserbycode(code:string) {
    return this.http.get<users>(this.baseUrl + 'User/GetBycode?code='+code);
  }

  Getallroles() {
    return this.http.get<roles[]>(this.baseUrl + 'UserRole/GetAllRoles');
  }

  Updaterole(_data: UpdateRole) {
    return this.http.post(this.baseUrl + 'User/updaterole', _data);
  }
  Updatestatus(_data: Updatestatus) {
    return this.http.post(this.baseUrl + 'User/updatestatus', _data);
  }

  Getallmenus() {
    return this.http.get<menus[]>(this.baseUrl + 'UserRole/GetAllMenus');
  }

  Assignrolepermission(_data: TblRolepermission[]) {
    return this.http.post(this.baseUrl + 'UserRole/asignrolepermission', _data);
  }




}
