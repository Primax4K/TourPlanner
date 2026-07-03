import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourListItem } from './tour-list-item';

describe('TourListItem', () => {
  let component: TourListItem;
  let fixture: ComponentFixture<TourListItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourListItem],
    }).compileComponents();

    fixture = TestBed.createComponent(TourListItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
