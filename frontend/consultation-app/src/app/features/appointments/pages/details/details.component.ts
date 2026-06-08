import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './details.component.html'
})
export class DetailsComponent implements OnInit {

  appointment: any;

  constructor(
    private route: ActivatedRoute,
    private service: AppointmentService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.service.getById(id).subscribe((res: any) => {
      this.appointment = res;
    });
  }
}