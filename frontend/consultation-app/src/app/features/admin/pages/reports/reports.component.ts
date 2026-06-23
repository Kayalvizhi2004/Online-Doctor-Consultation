import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportService } from '../../../../core/services/report.service';
import { AdminService } from '../../../../core/services/admin.service';

function toCsv(rows: any[], columns?: string[]) {
  if (!rows || !rows.length) return '';
  const keys = columns && columns.length ? columns : Array.from(new Set(rows.flatMap(r => Object.keys(r))));
  const esc = (v: any) => '"' + String(v ?? '').replace(/"/g, '""') + '"';
  const header = keys.map(k => esc(k)).join(',') + '\n';
  const body = rows.map(r => keys.map(k => esc(r[k])).join(',')).join('\n');
  return header + body;
}

function downloadFile(filename: string, content: string, mime = 'text/csv') {
  const blob = new Blob([content], { type: mime });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {

  report: any;
  appointmentsPerMonthData: any[] = [];
  doctorsBySpecializationData: any[] = [];
  mostActiveDoctorsData: any[] = [];
  mostActivePatientsData: any[] = [];
  revenueSummaryData: any[] = [];
  sessionCompletionRateData: any[] = [];

  constructor(private reportService: ReportService, private adminService: AdminService) {}

  ngOnInit(): void { this.loadReport(); }

  loadReport(): void {
    this.adminService.getKpis().subscribe((res: any) => this.report = res || {});

    this.reportService.appointmentsPerMonth().subscribe((r:any) => {
      this.appointmentsPerMonthData = normalizeListResponse(r);
      const vals = this.appointmentsPerMonthData.map(x => Number(x.count ?? x.value ?? x?.value ?? 0));
      this.appointmentsMax = vals.length ? Math.max(...vals) : 1;
      if (!this.appointmentsMax) this.appointmentsMax = 1;
    });
    this.reportService.doctorsBySpecialization().subscribe((r:any) => this.doctorsBySpecializationData = normalizeListResponse(r));
    this.reportService.mostActiveDoctors().subscribe((r:any) => this.mostActiveDoctorsData = normalizeListResponse(r));
    this.reportService.mostActivePatients().subscribe((r:any) => this.mostActivePatientsData = normalizeListResponse(r));
    this.reportService.revenueSummary().subscribe((r:any) => this.revenueSummaryData = normalizeListResponse(r));
    this.reportService.sessionCompletionRate().subscribe((r:any) => this.sessionCompletionRateData = normalizeListResponse(r));
  }
  appointmentsMax = 1;

  exportCsv(name: string, rows: any[], columns?: string[]) {
    const csv = toCsv(rows, columns);
    if (!csv) return;
    downloadFile(`${name.replace(/\s+/g,'_')}.csv`, csv, 'text/csv');
  }

  printSection(id: string) {
    const el = document.getElementById(id);
    if (!el) return;
    const w = window.open('', '_blank', 'noopener');
    if (!w) return;
    w.document.write(`<html><head><title>Report - ${id}</title>`);
    const styles = Array.from(document.querySelectorAll('style, link[rel="stylesheet"]')).map(s => s.outerHTML).join('\n');
    w.document.write(styles);
    w.document.write('</head><body>');
    w.document.write(el.innerHTML);
    w.document.write('</body></html>');
    w.document.close();
    w.focus();
    setTimeout(()=> w.print(), 300);
  }
}

function normalizeListResponse(res: any): any[] {
  if (!res) return [];
  if (Array.isArray(res)) return res;
  if (Array.isArray(res.items)) return res.items;
  if (Array.isArray(res.data)) return res.data;
  if (Array.isArray(res.data?.items)) return res.data.items;
  // If it's an object of primitive values, convert to array of { key, value }
  if (typeof res === 'object') {
    const entries = Object.entries(res).filter(([k,_v]) => k !== 'items' && k !== 'data');
    if (entries.length && entries.every(([,v]) => (typeof v === 'number' || typeof v === 'string'))) {
      return entries.map(([k,v]) => ({ name: k, value: v }));
    }
  }
  return [];
}