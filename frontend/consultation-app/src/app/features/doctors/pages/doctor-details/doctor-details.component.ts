import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DoctorService } from '../../../../core/services/doctor.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-doctor-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './doctor-details.component.html',
  styleUrls: ['./doctor-details.component.css']
})
export class DoctorDetailsComponent implements OnInit {

  doctor: any;

  constructor(
    private route: ActivatedRoute,
    private doctorService: DoctorService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadDoctor(id);
  }

  loadDoctor(id: string) {
    this.doctorService.getDoctorById(id).subscribe((res: any) => {
      this.doctor = res;
    });
  }
}