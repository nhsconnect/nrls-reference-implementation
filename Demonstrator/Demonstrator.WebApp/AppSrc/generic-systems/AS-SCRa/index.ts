import { BaseGenericSystem } from "../BaseGenericSystem";
import { IRequest } from "../../core/interfaces/IRequest";
import { IPointer } from "../../core/interfaces/IPointer";
import { IPointerDocument } from "../../core/interfaces/IPointerDocument";

export class GsASSCRa extends BaseGenericSystem {
  data: any = {};

  showSearch: boolean = true;
  showDetail: boolean = false;

  request?: IRequest;

  public pointers: IPointer[] | null = [];
  public currentPointer: IPointer | null = null;
  public pointerDocument?: IPointerDocument;

  activate(model) {
    this.data = model;
  }

  findPatient() {
    this.showSearch = true;
    this.showDetail = false;

    this.patient = undefined;
  }

  displayDetail() {
    this.showSearch = false;
    this.showDetail = true;
  }

  startView() {
    this.toggleInstructions();

    this.trackView(this.data.genericSystem.fModule, this.data.personnel.name);
  }

  private patientChanged(newValue: string, oldValue: string): void {
    if (!newValue) {
      return;
    }

    this.pointers = [];
    this.currentPointer = null;
    this.pointerDocument = undefined;

    this.createRequest();
    this.displayDetail();

    const tab = document.querySelector(
      "[role=tab][aria-controls=overview]"
    ) as HTMLElement;

    tab.click();
  }

  private createRequest(): void {
    this.request = <IRequest>{
      headers: {
        Asid: this.data.genericSystem.asid,
        OrgCode: this.data.organisation.orgCode,
      },
      id: this.patient ? this.patient.nhsNumber : null,
      active: true,
    };
  }

  deactivate() {
    this.data = {};
    this.pointerDocument = undefined;
  }
}
