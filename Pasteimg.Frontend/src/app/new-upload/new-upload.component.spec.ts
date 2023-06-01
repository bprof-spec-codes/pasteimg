import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewUploadComponent } from './new-upload.component';

describe('NewUploadComponent', () => {
  let component: NewUploadComponent;
  let fixture: ComponentFixture<NewUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NewUploadComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
