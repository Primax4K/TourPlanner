import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourToolbar } from './tour-toolbar';

describe('TourToolbar', () => {
  let component: TourToolbar;
  let fixture: ComponentFixture<TourToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourToolbar],
    }).compileComponents();

    fixture = TestBed.createComponent(TourToolbar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
