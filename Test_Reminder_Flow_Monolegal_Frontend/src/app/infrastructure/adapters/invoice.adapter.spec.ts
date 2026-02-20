import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { InvoiceAdapter } from './invoice.adapter';
import { environment } from '../../../environments/environment';

describe('InvoiceAdapter', () => {
  let adapter: InvoiceAdapter;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [InvoiceAdapter]
    });

    adapter = TestBed.inject(InvoiceAdapter);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(adapter).toBeTruthy();
  });

  it('should get all invoices', () => {
    const mockResponse = {
      status_code: 200,
      message: 'Success',
      data: {
        invoices: [],
        total: 0
      }
    };

    adapter.getAll().subscribe(response => {
      expect(response.status_code).toBe(200);
      expect(response.data?.invoices).toEqual([]);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/invoices`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get invoice by id', () => {
    const mockResponse = {
      status_code: 200,
      message: 'Success',
      data: {
        id: '123',
        invoice_number: 'INV-001'
      }
    };

    adapter.getById('123').subscribe(response => {
      expect(response.status_code).toBe(200);
      expect(response.data?.id).toBe('123');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/invoices/123`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should delete invoice', () => {
    const mockResponse = {
      status_code: 200,
      message: 'Deleted',
      data: null
    };

    adapter.delete('123').subscribe(response => {
      expect(response.status_code).toBe(200);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/invoices/123`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });
});
