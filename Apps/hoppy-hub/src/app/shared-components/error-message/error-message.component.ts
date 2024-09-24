import { Component, Input } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [FontAwesomeModule],
  templateUrl: './error-message.component.html'
})
export class ErrorMessageComponent {
  @Input({ required: true }) message!: string;
  faXmark = faXmark;
}
