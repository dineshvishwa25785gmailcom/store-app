import { TestBed } from '@angular/core/testing';

import { DecimalFormatterService } from './decimal-formatter.service';

describe('DecimalFormatterService', () => {
  let service: DecimalFormatterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DecimalFormatterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
