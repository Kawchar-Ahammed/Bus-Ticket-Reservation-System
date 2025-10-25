import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PaymentService } from '../../../services/payment.service';
import { 
  Payment, 
  PaymentMethod, 
  PaymentGateway, 
  CreatePaymentRequest,
  getPaymentMethodName,
  getPaymentGatewayName,
  getPaymentStatusName,
  getPaymentStatusColor
} from '../../../models/payment.model';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-payment-processing',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './payment-processing.html',
  styleUrl: './payment-processing.scss',
})
export class PaymentProcessing implements OnInit {
  @Input() ticketId!: string;
  @Input() amount!: number;
  @Input() customerEmail!: string;
  @Output() paymentCompleted = new EventEmitter<Payment>();
  @Output() paymentFailed = new EventEmitter<string>();
  @Output() cancelled = new EventEmitter<void>();

  paymentForm!: FormGroup;
  isProcessing = false;
  showResult = false;
  paymentResult: Payment | null = null;

  // Enums for template
  PaymentMethod = PaymentMethod;
  PaymentGateway = PaymentGateway;

  // Payment method options
  paymentMethods = [
    { value: PaymentMethod.Card, label: 'Credit/Debit Card', icon: 'bi-credit-card' },
    { value: PaymentMethod.Cash, label: 'Cash on Pickup', icon: 'bi-cash' },
    { value: PaymentMethod.BankTransfer, label: 'Bank Transfer', icon: 'bi-bank' },
    { value: PaymentMethod.BKash, label: 'bKash', icon: 'bi-phone' },
    { value: PaymentMethod.Nagad, label: 'Nagad', icon: 'bi-phone' },
    { value: PaymentMethod.Rocket, label: 'Rocket', icon: 'bi-phone' }
  ];

  // Payment gateway options (filtered based on method)
  paymentGateways = [
    { value: PaymentGateway.Mock, label: 'Mock Gateway (Testing)', methods: [PaymentMethod.Card, PaymentMethod.BankTransfer] },
    { value: PaymentGateway.SSLCommerz, label: 'SSLCommerz', methods: [PaymentMethod.Card, PaymentMethod.BankTransfer] },
    { value: PaymentGateway.Stripe, label: 'Stripe', methods: [PaymentMethod.Card] },
    { value: PaymentGateway.BKash, label: 'bKash Payment', methods: [PaymentMethod.BKash] },
    { value: PaymentGateway.Nagad, label: 'Nagad Payment', methods: [PaymentMethod.Nagad] },
    { value: PaymentGateway.Rocket, label: 'Rocket Payment', methods: [PaymentMethod.Rocket] },
    { value: PaymentGateway.Manual, label: 'Manual Processing', methods: [PaymentMethod.Cash, PaymentMethod.BankTransfer] }
  ];

  constructor(
    private fb: FormBuilder,
    private paymentService: PaymentService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.paymentForm = this.fb.group({
      paymentMethod: [PaymentMethod.BKash, Validators.required],
      paymentGateway: [PaymentGateway.Mock, Validators.required],
      agreeToTerms: [false, Validators.requiredTrue]
    });

    // Auto-select gateway when method changes
    this.paymentForm.get('paymentMethod')?.valueChanges.subscribe(method => {
      this.onPaymentMethodChange(method);
    });
  }

  onPaymentMethodChange(method: PaymentMethod): void {
    // Auto-select appropriate gateway
    const availableGateways = this.getAvailableGateways(method);
    if (availableGateways.length > 0) {
      this.paymentForm.patchValue({ paymentGateway: availableGateways[0].value });
    }
  }

  getAvailableGateways(method: PaymentMethod) {
    return this.paymentGateways.filter(g => g.methods.includes(method));
  }

  get currentGateways() {
    const method = this.paymentForm.get('paymentMethod')?.value;
    return this.getAvailableGateways(method);
  }

  processPayment(): void {
    if (this.paymentForm.invalid) {
      this.toastService.showError('Please fill in all required fields');
      return;
    }

    this.isProcessing = true;
    this.showResult = false;

    const request: CreatePaymentRequest = {
      ticketId: this.ticketId,
      amount: this.amount,
      paymentMethod: this.paymentForm.value.paymentMethod,
      paymentGateway: this.paymentForm.value.paymentGateway,
      customerEmail: this.customerEmail
    };

    this.paymentService.processPayment(request).subscribe({
      next: (payment) => {
        this.isProcessing = false;
        this.paymentResult = payment;
        this.showResult = true;

        if (payment.status === 2) { // Completed
          this.paymentCompleted.emit(payment);
        } else if (payment.status === 3) { // Failed
          this.paymentFailed.emit(payment.failureReason || 'Payment processing failed');
        }
      },
      error: (error) => {
        this.isProcessing = false;
        this.paymentFailed.emit(error.message || 'Payment processing failed');
      }
    });
  }

  retryPayment(): void {
    this.showResult = false;
    this.paymentResult = null;
  }

  cancel(): void {
    this.cancelled.emit();
  }

  // Helper methods for template
  getMethodName(method: PaymentMethod): string {
    return getPaymentMethodName(method);
  }

  getGatewayName(gateway: PaymentGateway): string {
    return getPaymentGatewayName(gateway);
  }

  getStatusName(status: number): string {
    return getPaymentStatusName(status);
  }

  getStatusColor(status: number): string {
    return getPaymentStatusColor(status);
  }
}
