import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListinvoiceComponent } from './listinvoice.component';

describe('ListinvoiceComponent', () => {
  let component: ListinvoiceComponent;
  let fixture: ComponentFixture<ListinvoiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListinvoiceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListinvoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
