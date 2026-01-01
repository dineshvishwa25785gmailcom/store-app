import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root', // Makes the service available everywhere
})
export class DecimalFormatterService {
  // Function to round to 2 decimal places
  roundToTwoDecimal(value: number): number {
    return Math.round(value * 100) / 100;
  }

  // Function to round to 3 decimal places
  roundToThreeDecimal(value: number): number {
    return Math.round(value * 1000) / 1000;
  }

  // Function to restrict input to 3 decimal places in real-time
  restrictDecimals(event: any) {
    let value = event.target.value;
  
    // Remove any non-numeric characters except ONE decimal point
    event.target.value = value.replace(/[^0-9.]/g, '');
  
    // Prevent multiple decimal points
    if ((event.target.value.match(/\./g) || []).length > 1) {
      event.target.value = event.target.value.replace(/(\..*?)\..*/g, '$1'); // Allows only ONE dot
      return;
    }
  
    // Restrict to 3 decimal places
    if (event.target.value.includes('.')) {
      const [integerPart, decimalPart] = event.target.value.split('.');
      if (decimalPart?.length > 3) {
        event.target.value = `${integerPart}.${decimalPart.substring(0, 3)}`;
      }
    }
  }

  // NEW FUNCTION: Handle when user leaves the input field
  formatOnBlur(event: any) {
    let value = event.target.value.trim();
    console.log("Before formatting:", value);  // Debugging line
  
    if (!isNaN(parseFloat(value))) {
      if (value.endsWith('.')) {
        event.target.value = value + '000';
      } else {
        event.target.value = parseFloat(value).toFixed(3);
      }
    }
    console.log("After formatting:", event.target.value);  // Debugging line
  }
  
}
