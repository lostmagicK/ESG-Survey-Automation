import { Component } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { AnswerModel, SurveyService } from '../../../build';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  constructor(private formBuilder: FormBuilder, private service: SurveyService, private snackBar: MatSnackBar, private router: Router) {
    this.question = this.formBuilder.control({disabled: false, value: ''}, {validators: [Validators.required]});
  }
  loading = false;
  question: FormControl;
  uploadPdf = false;
  answer: AnswerModel | null | undefined;
  onToggle(uploadPdf: boolean) {
    this.uploadPdf = uploadPdf;
  }
  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }
  onTrainingFileSelected(event: any) {
    this.selectedTrainingFile = event.target.files[0];
  }
  selectedFile: File | null = null;
  selectedTrainingFile: File | null = null;
  uploadQuestionAir() {
    if (!this.selectedFile) {
      this.snackBar.open(`No file selected.`, undefined, {duration: 2000, verticalPosition: 'top'});
      return;
    }
    this.loading = true;
    this.service.apiSurveyUploadSurveyQuestionAirPost(this.selectedFile).subscribe(
      response => {
        this.loading = false;
        this.selectedFile = null;
        this.snackBar.open(`File uploaded successfully`, undefined, {duration: 2000, verticalPosition: 'top'});
      },
      (err: HttpErrorResponse) => {
        this.loading = false;
        if(err.status === 401){
          localStorage.clear();
          this.router.navigate(['/sign-in']);
        }
        this.snackBar.open(`API (${err.status}) Error: ${err.error}`, undefined, {duration: 2000, verticalPosition: 'top'});
      }
    );
  }
  uploadTrainingDocument() {
    if (!this.selectedTrainingFile) {
      this.snackBar.open(`No file selected.`, undefined, {duration: 2000, verticalPosition: 'top'});
      return;
    }
    this.loading = true;
    this.service.apiSurveyUploadTraingDocumentPost(this.selectedTrainingFile).subscribe(
      response => {
        this.loading = false;
        this.selectedTrainingFile = null;
        this.snackBar.open(`File uploaded successfully`, undefined, {duration: 2000, verticalPosition: 'top'});
      },
      (err: HttpErrorResponse) => {
        this.loading = false;
        if(err.status === 401){
          localStorage.clear();
          this.router.navigate(['/sign-in']);
        }
        this.snackBar.open(`API (${err.status}) Error: ${err.error}`, undefined, {duration: 2000, verticalPosition: 'top'});
      }
    );
  }

  askQuestion() {
    if (this.question.invalid) {
      this.snackBar.open(`Question is required`, undefined, {duration: 2000, verticalPosition: 'top'});
      return;
    }
    this.loading = true;
    this.service.apiSurveyAskQuestionGet(this.question.value).subscribe(data => {
      this.answer = data;
      this.loading = false;
    }, (err: HttpErrorResponse) => {
      this.loading = false;
      if(err.status === 401){
        localStorage.clear();
        this.router.navigate(['/sign-in']);
      }
      this.snackBar.open(`API (${err.status}) Error: ${err.error}`, undefined, {duration: 2000, verticalPosition: 'top'});
    });
  }
}
