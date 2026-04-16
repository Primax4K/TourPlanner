import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateLog } from './create-log';

describe('CreateLog', () => {
  let component: CreateLog;
  let fixture: ComponentFixture<CreateLog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateLog],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateLog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
