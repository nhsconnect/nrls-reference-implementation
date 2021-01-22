import { PatientSvc } from "../core/services/PatientService";
import { bindable, inject, observable, bindingMode } from "aurelia-framework";
import { IPatientNumber } from "../core/interfaces/IPatientNumber";
import { IPatient } from "../core/interfaces/IPatient";
import { EprSvc } from "../core/services/EprService";
import { AnalyticsSvc } from "../core/services/AnalyticsService";
import { IPointerDocument } from "../core/interfaces/IPointerDocument";
import { IPointer } from "../core/interfaces/IPointer";

@inject(PatientSvc, EprSvc, AnalyticsSvc)
export class BaseGenericSystem {
  @observable
  protected patient?: IPatient;

  patientLoading: boolean = false;
  instructionsActive: boolean = true;

  @observable
  selectedPatient?: IPatientNumber;

  @observable
  pointerDocumentLoaded?: boolean;

  pointerDocumentViewer: any;

  pointerDocument?: IPointerDocument;

  showSystemMessage: boolean = false;

  systemMessage?: string = "Unknown";

  constructor(
    private patientSvc: PatientSvc,
    protected eprSvc: EprSvc,
    private analyticsSvc: AnalyticsSvc
  ) {
    this.escListener = this.escListener.bind(this);
  }

  escListener(e: KeyboardEvent) {
    if (e.key === "Escape") {
      this.instructionsActive = false;
    }
  }

  attached() {
    $('[data-toggle="tooltip"]').tooltip();

    window.addEventListener("keydown", this.escListener);
  }

  toggleInstructions() {
    this.instructionsActive = !this.instructionsActive;
  }

  getPatient() {
    if (
      !this.selectedPatient ||
      !this.selectedPatient.nhsNumber ||
      this.selectedPatient.id.length === 0
    ) {
      return;
    }

    this.patientLoading = true;
    this.patientSvc
      .getPatient(`${this.selectedPatient.nhsNumber}`)
      .then((patient) => {
        this.patient = patient;
        this.patientLoading = false;
      });
  }

  findPatient() {
    this.patient = undefined;
  }

  trackView(system: string, whos: string) {
    this.analyticsSvc.genericSystems(system, whos);
  }

  protected setSystemMessage(msg?: string) {
    this.showSystemMessage = !this.showSystemMessage;
    this.systemMessage = msg;
  }

  private selectedPatientChanged(newValue: string, oldValue: string): void {
    if (newValue === oldValue) {
      return;
    }

    this.getPatient();
  }

  private closePointerDocument(): void {
    this.pointerDocument = undefined;
  }

  private pointerDocumentLoadedChanged(
    newValue?: boolean,
    oldValue?: boolean
  ): void {
    if (newValue === true) {
      this.pointerDocumentViewer.loadDocument();
    }
  }

  get showPatientSearch(): boolean {
    return !this.patient;
  }

  detached() {
    $('[data-toggle="tooltip"]').tooltip("dispose");

    window.removeEventListener("keydown", this.escListener);
  }
}
