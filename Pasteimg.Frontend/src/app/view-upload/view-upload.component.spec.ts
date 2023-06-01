import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewUploadComponent } from './view-upload.component';

describe('ViewUploadComponent', () => {
  let component: ViewUploadComponent;
  let fixture: ComponentFixture<ViewUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewUploadComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
