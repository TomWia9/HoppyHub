import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [],
  templateUrl: './error-message.component.html'
})
export class ErrorMessageComponent {
  @Input({ required: true }) message!: string;
}
