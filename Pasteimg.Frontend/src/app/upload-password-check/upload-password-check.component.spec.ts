import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadPasswordCheckComponent } from './upload-password-check.component';

describe('UploadPasswordCheckComponent', () => {
  let component: UploadPasswordCheckComponent;
  let fixture: ComponentFixture<UploadPasswordCheckComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadPasswordCheckComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UploadPasswordCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
