export * from './accounts.service';
import { AccountsService } from './accounts.service';
export * from './survey.service';
import { SurveyService } from './survey.service';
export const APIS = [AccountsService, SurveyService];
