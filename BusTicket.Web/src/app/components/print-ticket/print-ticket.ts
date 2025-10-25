import { Component, OnInit, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { BusApi } from '../../services/bus-api';
import * as QRCode from 'qrcode';

@Component({
  selector: 'app-print-ticket',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule
  ],
  templateUrl: './print-ticket.html',
  styleUrl: './print-ticket.scss',
})
export class PrintTicket implements OnInit, AfterViewInit {
  @ViewChild('qrCanvas', { static: false }) qrCanvas?: ElementRef<HTMLCanvasElement>;
  
  ticket: any = null;
  loading = true;
  qrCodeData = '';

  constructor(
    private route: ActivatedRoute,
    private busApi: BusApi
  ) {}

  ngOnInit() {
    const ticketNumber = this.route.snapshot.paramMap.get('ticketNumber');
    if (ticketNumber) {
      this.qrCodeData = ticketNumber;
      this.loadTicket(ticketNumber);
    }
  }

  ngAfterViewInit() {
    if (this.qrCodeData && this.qrCanvas) {
      this.generateQRCode();
    }
  }

  loadTicket(ticketNumber: string) {
    this.busApi.getBookingByTicket(ticketNumber).subscribe({
      next: (ticket) => {
        this.ticket = ticket;
        this.loading = false;
        // Generate QR code after ticket is loaded
        setTimeout(() => this.generateQRCode(), 100);
      },
      error: (error) => {
        console.error('Error loading ticket:', error);
        this.loading = false;
      }
    });
  }

  generateQRCode() {
    if (this.qrCanvas && this.qrCodeData) {
      const canvas = this.qrCanvas.nativeElement;
      QRCode.toCanvas(canvas, this.qrCodeData, {
        width: 150,
        margin: 1,
        color: {
          dark: '#000000',
          light: '#FFFFFF'
        }
      }, (error: any) => {
        if (error) {
          console.error('QR Code generation error:', error);
        }
      });
    }
  }

  print() {
    window.print();
  }

  getCurrentDate(): Date {
    return new Date();
  }
}
