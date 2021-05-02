import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { of } from 'rxjs';
import { DepartmentListComponent } from './dep-list.component';
import { SharedService } from '../../../services/shared/shared.service';
import { Department } from '../department.component';

describe('DepartmentListComponent', () => {
  let component: DepartmentListComponent;
  let fixture: ComponentFixture<DepartmentListComponent>;
  let service: SharedService;
  let mockList: Department[];
  let mock: Department;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DepartmentListComponent],
      imports: [HttpClientModule, FormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
      .compileComponents();
  })

  beforeEach(() => {
    fixture = TestBed.createComponent(DepartmentListComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get<SharedService>(SharedService as any);
    mockList = [];
    mock = <Department>{DepartmentId: 0, DepartmentName: ''};
    fixture.detectChanges();
  })

  it('should create department list component', () => {
    expect(component).toBeTruthy();
  })

  it('should deactivate department modal component when initialize department list component', () => {
    expect(component.activateDepModalComp).toBeFalse();
  })

  it('should set department filter id value when initialize department list component', () => {
    expect(component.departmentIdFilter).toEqual('');
  })

  it('should set department filter name value when initialize department list component', () => {
    expect(component.departmentNameFilter).toEqual('');
  })

  it('should set call update department list when initialize department list component', () => {
    const spy = spyOn(component, 'updateDepartmentList');
    component.ngOnInit();
    expect(spy).toHaveBeenCalled();
  })

  it('should call shared service when update department list', () => {
    const spy = spyOn(service, 'getDepartmentListFromDB').and.returnValue(of(mockList));
    component.updateDepartmentList();
    expect(spy.calls.any()).toBeTruthy();
  })

  it('should set department list value when update department list', () => {
    spyOn(service, 'getDepartmentListFromDB').and.returnValue(of(mockList));
    component.updateDepartmentList();
    expect(component.departmentList).toEqual(mockList);
  })

  it('should call add department method when click on button', () => {
    const spy = spyOn(component, 'addDepartment');
    const btn = fixture.debugElement.query(By.css('.btn-float'));
    btn.triggerEventHandler('click', null);
    expect(spy).toHaveBeenCalled();
  })

  it('should activate modal department component when add department', () => {
    component.addDepartment();
    expect(component.activateDepModalComp).toBeTrue();
  })

  it('should set department object values when add department', () => {
    component.addDepartment();
    expect(component.department).toEqual(mock);
  })

  it('should have modal title value as `Add Department` when add department', () => {
    component.addDepartment();
    expect(component.modalTitle).toEqual('Add Department');
  })

  it('should activate department modal component when edit department', () => {
    component.editDepartment(mock);
    expect(component.activateDepModalComp).toBeTrue();
  })

  it('should set department object value when edit department', () => {
    component.editDepartment(mock);
    expect(component.department).toEqual(mock);
  })
})
