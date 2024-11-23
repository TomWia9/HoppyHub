import { ModalType } from './model-type';

export class ModalModel {
  constructor(
    public modalType: ModalType,
    public modalData: Record<string, unknown> = {}
  ) {}
}
