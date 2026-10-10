import { Component, EventEmitter, input, Input, Output } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { PagedData } from '../../types/paged-data';

@Component({
  selector: 'app-table',
  imports: [MatTableModule, MatCardModule, MatButtonModule, MatPaginatorModule],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss',
})
export class TableComponent {
  @Input() pagedData: PagedData<any> = {
  data: [],
  totalData: 0
};

  @Input() displayedColumns: any[] = [];
  @Output() onEdit = new EventEmitter<any>();
  @Output() onDelete = new EventEmitter<any>();
  @Output() onPageChange = new EventEmitter<any>();
  @Output() rowClick = new EventEmitter<any>();
  @Input() pageIndex!: number;
  @Input() pageSize!: number;
  @Input() showPage= true;
  cols: any[] = [];
  ngOnInit() {
    this.cols = this.displayedColumns.map((x) => x.key || x);
  }
  edit(rowData: any) {
    this.onEdit.emit(rowData);
  }
  delete(rowData: any) {
    this.onDelete.emit(rowData);
  }
  pageChange(event: any) {
    console.log(event);
    this.onPageChange.emit(event);
  }
  onButtonClick(btn: string, rowData: any) {
    this.rowClick.emit({
      btn,
      rowData,
    });
  }
}
