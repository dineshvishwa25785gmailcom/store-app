export interface customer {
  uniqueKeyID: string;
  name: string;
  email: string;
  phone: string;
  isActive: boolean;
  addressDetails: string;
  countryCode: string;
  countryName: string;
  stateCode: string;
  stateName: string;
  mobileNo: string;
  alternateMobile: string;
  customer_company: string;
  gst_number: string;
  updateDate: string;
  createIp: string;
  updateIp: string;
  statusname?: string; // optional if not used in API
}