import { Pipe, PipeTransform } from '@angular/core';
import type { TaskItemStatus } from '../models/api.models';

const LABELS: Record<TaskItemStatus, string> = {
  Todo: 'To Do',
  InProgress: 'In Progress',
  Done: 'Done',
};

@Pipe({
  name: 'taskStatusLabel',
  standalone: true,
})
export class TaskStatusLabelPipe implements PipeTransform {
  transform(value: TaskItemStatus | string | null | undefined): string {
    if (value == null || value === '') {
      return '';
    }
    return LABELS[value as TaskItemStatus] ?? String(value);
  }
}
