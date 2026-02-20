import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StatusBadgeComponent } from './status-badge.component';

describe('StatusBadgeComponent', () => {
  let component: StatusBadgeComponent;
  let fixture: ComponentFixture<StatusBadgeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusBadgeComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(StatusBadgeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should return correct text for primerrecordatorio', () => {
    component.status = 'primerrecordatorio';
    expect(component.getStatusText()).toBe('Primer Recordatorio');
  });

  it('should return correct text for segundorecordatorio', () => {
    component.status = 'segundorecordatorio';
    expect(component.getStatusText()).toBe('Segundo Recordatorio');
  });

  it('should return correct text for desactivado', () => {
    component.status = 'desactivado';
    expect(component.getStatusText()).toBe('Desactivado');
  });

  it('should return correct variant for primerrecordatorio', () => {
    component.status = 'primerrecordatorio';
    expect(component.getVariant()).toBe('primer');
  });

  it('should return correct variant for segundorecordatorio', () => {
    component.status = 'segundorecordatorio';
    expect(component.getVariant()).toBe('segundo');
  });

  it('should return correct variant for desactivado', () => {
    component.status = 'desactivado';
    expect(component.getVariant()).toBe('desactivado');
  });
});
