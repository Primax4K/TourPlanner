import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourLogListItem } from './tour-log-list-item';

describe('TourLogListItem', () => {
  let component: TourLogListItem;
  let fixture: ComponentFixture<TourLogListItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourLogListItem],
    }).compileComponents();

    fixture = TestBed.createComponent(TourLogListItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
