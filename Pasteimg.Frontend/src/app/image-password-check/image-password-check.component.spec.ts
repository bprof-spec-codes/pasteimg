import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImagePasswordCheckComponent } from './image-password-check.component';

describe('ImagePasswordCheckComponent', () => {
  let component: ImagePasswordCheckComponent;
  let fixture: ComponentFixture<ImagePasswordCheckComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImagePasswordCheckComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImagePasswordCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
