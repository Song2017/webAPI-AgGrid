var agPagination = {
    PageIndex: 0,
    PageSize: 10
};

// Columns
var columnDefs = [
    {
        headerName: "", width: 84, field: "operatecol", hide: false,
        lockPosition: true, lockPinned: true, cellStyle: { padding: '0 2px' },
        lockVisible: true, pinned: "left", resizable: false, sortable: false,
        filter: false, suppressMenu: true, suppressMovable: true,
        suppressNavigable: true, pinnedRowCellRenderer: true,
        cellRenderer: 'dataItemTemplateRender',
        headerComponent: HeaderCheckbox,
        checkboxSelection: function (params) {
            return params.columnApi.getRowGroupColumns().length === 0;
        }
    },
    {
        headerName: "UNIQUEKEY", field: "uniquekey", filter: 'agTextColumnFilter',
        filterParams: {
            newRowsAction: 'keep', defaultOption: 'startsWith',
            applyButton: true, clearButton: true
        }
    },
    {
        headerName: "TAGNUMBER11", field: "tagnumber", sortable: true, filter: 'agTextColumnFilter',
        enableRowGroup: true, floatingFilterComponentParams: { debounceMs: 2000 }
    },
    { headerName: "Date Tested", field: "datetested", filter: 'agDateColumnFilter' },
    { headerName: "PLANTKEY", field: "plantkey", enableRowGroup: true },
    {
        headerName: "LOOPNUMBER", field: "loopnumber", filter: 'agNumberColumnFilter',
        enableRowGroup: true, enableValue: true
    },
    { headerName: "DATETESTEDSORT", field: "datetestedsort", enableRowGroup: true }
];

var gridOptions = {
    defaultColDef: {
        width: 180,// set every column width
        editable: false,// make every column editable
        enablePivot: false,
        enableRowGroup: false,
        enableSorting: true,
        filter: true,// enable filter sort
        sortable: true, //enable server sort
        resizable: true,
        suppressNavigable: true,
        menuTabs: ['filterMenuTab'],
        filterParams: { newRowsAction: 'keep' }
    },
    floatingFilter: true,
    //autoGroupColumnDef: groupColumn,
    columnDefs: columnDefs,
    multiSortKey: 'ctrl',
    debug: false,

    rowModelType: 'serverSide',
    rowSelection: 'multiple',
    rowDeselection: true,

    //cacheBlockSize:pagesize, in case no/duplicate data to show
    cacheBlockSize: 10,//Lazy-loading: with the grid options property: cacheBlockSize = 100 data will be fetched in blocks of 100 rows at a time.
    maxBlocksInCache: 1000,//to limit the amount of data cached in the grid you can set maxBlocksInCache via the gridOptions.
    infiniteInitialRowCount: 5,//How many rows to initially allow the user to scroll to. 
    //animateRows: true,
    pagination: true,
    paginationPageSize: 10,
    //quickFilterText: null,

    onPaginationChanged: onPaginationChanged,
    onRowSelected: rowSelected, //callback when row selected
    onSelectionChanged: selectionChanged, //callback when selection changed,
    sideBar: {
        toolPanels: [{
            id: 'columns',
            labelDefault: 'Columns',
            labelKey: 'columns',
            iconKey: 'columns',
            toolPanel: 'agColumnsToolPanel',
            toolPanelParams: {
                suppressPivots: true,
                suppressPivotMode: true
            }
        }]
    },

    components: {
        booleanCellRenderer: booleanCellRenderer,
        dataItemTemplateRender: DataItemTemplateRender,
    },
};

// setup the grid after the page has finished loading
document.addEventListener('DOMContentLoaded', function () {
    var gridDiv = document.querySelector('#myGrid');
    if (sessionStorage.getItem("PageSize")) {
        var sessionPageSize = Number(sessionStorage.getItem("PageSize"));
        gridOptions.cacheBlockSize = sessionPageSize;
        document.getElementById('selPageSize').value = sessionPageSize;
        gridOptions.paginationPageSize = sessionPageSize;
        agPagination.PageSize = sessionPageSize;
    }

    new agGrid.Grid(gridDiv, gridOptions);

    fetch('https://localhost:44364/api/aggrid/GetDataColumns/uspGetCVList').then(function (response) {
        return response.json();
    }).then(function (data) {
        gridOptions.api.setColumnDefs(columnDefs.concat(JSON.parse(data)));
    })
    gridOptions.api.setServerSideDatasource(new EnterpriseDatasource());
});


// Datasource
function EnterpriseDatasource() {
    EnterpriseDatasource.prototype.getRows = function (params) {
        var request = params.request;
        request['pageIndex'] = agPagination.PageIndex;
        request['pageSize'] = agPagination.PageSize;
        request['filterModel'] = formatFilterModel(request.filterModel);
        requestForServer = JSON.stringify(request, null, 2);

        var httpRequest = new XMLHttpRequest();
        httpRequest.open('POST', 'https://localhost:44364/api/aggrid/GetAllData');
        httpRequest.setRequestHeader("Content-type", "application/json");
        httpRequest.send(requestForServer);
        httpRequest.onreadystatechange = function () {
            if (httpRequest.readyState == 4 && httpRequest.status == 200) {
                httpResponseForServer = JSON.parse(httpRequest.responseText);
                params.successCallback(httpResponseForServer.data, httpResponseForServer.lastRow);
            }
        };
    };
}


// Functions
// SelectedRows
function getSelectedRows() {
    var selectedNodes = gridOptions.api.getSelectedNodes();
    var selecteddata = selectedNodes.map(function (node) {
        return node.data
    }).map(function (node) {
        return node.uniquekey + '_' + node.tagnumber
    }).join(', ');
    alert('selectNodes: ' + selecteddata);
}
// Column sizeToFit
function sizeToFit() {
    gridOptions.api.sizeColumnsToFit();
}
// Column autoSizeAll
function autoSizeAll() {
    var allColumnIds = [];
    gridOptions.columnApi.getAllColumns().forEach(function (column) {
        if (column.colId != 'selectcol' && column.colId != 'operatecol')
            allColumnIds.push(column.colId);
    });
    gridOptions.columnApi.autoSizeColumns(allColumnIds.slice(1));
}
// Export
function onBtExport() {
    var params = {
        skipHeader: false,
        columnGroups: false,
        allColumns: true,
        onlySelected: true,
        fileName: "export_filename",
        sheetName: "export_sheet"
    };

    params.processCellCallback = function (params) {
        if (params.value && params.value.toUpperCase) {
            return params.value.toUpperCase();
        } else {
            return params.value;
        }
    }

    params.processHeaderCallback = function (params) {
        return params.column.getColDef().headerName.toUpperCase();
    };

    gridOptions.api.exportDataAsExcel(params);
}
// Print
function onBtPrint() {
    var gridApi = gridOptions.api;
    setPrinterFriendly(gridApi);

    setTimeout(function () {
        print();
        setNormal(gridApi);
    }, 2000);
}
function setPrinterFriendly(api) {
    var eGridDiv = document.querySelector('.my-grid');
    eGridDiv.style.width = '';
    eGridDiv.style.height = '';

    api.setDomLayout('print');
}
function setNormal(api) {
    var eGridDiv = document.querySelector('.my-grid');
    eGridDiv.style.width = '1200px';
    eGridDiv.style.height = '540px';

    api.setDomLayout(null);
}
// Pagination
function onPageSizeChanged() {
    var value = Number(document.getElementById('selPageSize').value);
    sessionStorage.setItem("PageSize", value);
    location.reload();
}
function onPaginationNext() {
    gridOptions.api.paginationGoToNextPage();
    agPagination.PageIndex = gridOptions.api.paginationGetCurrentPage();
}
function onPaginationChanged() {
    if (gridOptions.api) {
        agPagination.PageIndex = gridOptions.api.paginationGetCurrentPage();
    }
}
// Filter
function formatFilterModel(filterModels) {
    var aryFilter = [];
    var objChild = {};
    var aryCondition = [];
    for (var filter in filterModels) {
        if (filterModels[filter].operator) {
            objChild["head"] = { "field": filter, "operate": filterModels[filter].operator };

            if (filterModels[filter].condition1)
                aryCondition.push(filterModels[filter].condition1);
            if (filterModels[filter].condition2)
                aryCondition.push(filterModels[filter].condition2);
        }
        else {
            objChild["head"] = { "field": filter, "operate": "" };
            aryCondition.push(filterModels[filter]);
        }
        objChild["condition"] = aryCondition;
        aryFilter.push(objChild);
        aryCondition = [];
        objChild = {};
    }

    return aryFilter;
}
// Row 
function rowSelected(event) {
    rowSelectedevent = event;
    console.log('rowSelected:' + event.data.tagnumber);
}
function selectionChanged(event) {
    console.log('selectionChanged:');
    selectionChangedevent = event;
}
// Render
function booleanCellRenderer(params) {
    var valueCleaned = params.value;
    if (valueCleaned === 'T') {
        return '<input type="checkbox" checked/>';
    } else if (valueCleaned === 'F') {
        return '<input type="checkbox" unchecked/>';
    } else if (params.value !== null && params.value !== undefined) {
        return params.value.toString();
    } else {
        return null;
    }
}
function DataItemTemplateRender() { }
DataItemTemplateRender.prototype.init = function (params) {
    this.eGui = document.createElement("div");
    this.eGui.className = "clsDataItemTemplate";
    let aElement = document.createElement("a");
    aElement.setAttribute('rel', "noopener noreferrer");
    aElement.setAttribute('target', "_blank");
    aElement.className = "clsCommon clsview";
    aElement.href = 'newtab?' + params.data.uniquekey;
    this.eGui.appendChild(aElement);

    aElement = document.createElement("a");
    aElement.setAttribute('target', "_blank");
    aElement.className = "clsCommon clsedit";
    aElement.onclick = function () {
        window.open('newtabedit?' + params.data.uniquekey, params.data.uniquekey + 'Edit')
    };
    this.eGui.appendChild(aElement);

    aElement = document.createElement("a");
    aElement.className = "clsCommon clsdelete";
    aElement.onclick = function () { alert('delete'); };
    this.eGui.appendChild(aElement);
}
DataItemTemplateRender.prototype.getGui = function () {
    return this.eGui;
};
// Header Components
function HeaderCheckbox() { }
HeaderCheckbox.prototype.init = function (agParams) {
    agagParams = agParams;
    this.agParams = agParams;
    this.eGui = document.createElement('div');
    this.eGui.className = "clsDataItemTemplate";
    this.eHeaderCheckBox = document.createElement("input");
    this.eHeaderCheckBox.setAttribute('type', 'checkbox');
    this.onHeaderCheckBoxChangedListener = this.onHeaderCheckBoxChanged.bind(this);
    this.eHeaderCheckBox.addEventListener('change', this.onHeaderCheckBoxChangedListener);

    this.eHeaderNewButton = document.createElement("a");
    this.eHeaderNewButton.setAttribute('rel', "noopener noreferrer");
    this.eHeaderNewButton.setAttribute('target', "_blank");
    this.eHeaderNewButton.className = "clsCommon clsadd";
    this.eHeaderNewButton.href = 'addnew';

    this.eGui.appendChild(this.eHeaderCheckBox);
    this.eGui.appendChild(this.eHeaderNewButton);
}
HeaderCheckbox.prototype.getGui = function () {
    return this.eGui;
};
HeaderCheckbox.prototype.onHeaderCheckBoxChanged = function () {
    if (this.eHeaderCheckBox.checked == true) {
        gridOptions.api.forEachNode(node => node.setSelected(true));
    } else {
        gridOptions.api.deselectAll();
    }
};
HeaderCheckbox.prototype.destroy = function () {
    this.agParams.column.removeEventListener('change', this.onHeaderCheckBoxChangedListener);
};

function CustomHeader() { }
CustomHeader.prototype.init = function (agParams) {
    this.agParams = agParams;
    this.eGui = document.createElement('div');
    this.eGui.innerHTML = '' +
        '<div class="customHeaderMenuButton"><i class="fa ' + this.agParams.menuIcon + '"></i></div>' +
        '<div class="customHeaderLabel">' + this.agParams.displayName + '</div>' +
        '<div class="customSortDownLabel inactive"><i class="fa fa-long-arrow-alt-down"></i></div>' +
        '<div class="customSortUpLabel inactive"><i class="fa fa-long-arrow-alt-up"></i></div>' +
        '<div class="customSortRemoveLabel inactive"><i class="fa fa-times"></i></div>' +
        '<input class="customHeaderCheckBox" type="checkbox">';

    this.eMenuButton = this.eGui.querySelector(".customHeaderMenuButton");
    this.eSortDownButton = this.eGui.querySelector(".customSortDownLabel");
    this.eSortUpButton = this.eGui.querySelector(".customSortUpLabel");
    this.eSortRemoveButton = this.eGui.querySelector(".customSortRemoveLabel");
    this.eHeaderCheckBox = this.eGui.querySelector(".customHeaderCheckBox");

    this.onHeaderCheckBoxChangedListener = this.onHeaderCheckBoxChanged.bind(this);
    this.eHeaderCheckBox.addEventListener('change', this.onHeaderCheckBoxChangedListener)

    if (this.agParams.enableMenu) {
        this.onMenuClickListener = this.onMenuClick.bind(this);
        this.eMenuButton.addEventListener('click', this.onMenuClickListener);
    } else {
        this.eGui.removeChild(this.eMenuButton);
    }

    if (this.agParams.enableSorting) {
        this.onSortAscRequestedListener = this.onSortRequested.bind(this, 'asc');
        this.eSortDownButton.addEventListener('click', this.onSortAscRequestedListener);
        this.onSortDescRequestedListener = this.onSortRequested.bind(this, 'desc');
        this.eSortUpButton.addEventListener('click', this.onSortDescRequestedListener);
        this.onRemoveSortListener = this.onSortRequested.bind(this, '');
        this.eSortRemoveButton.addEventListener('click', this.onRemoveSortListener);

        this.onSortChangedListener = this.onSortChanged.bind(this);
        this.agParams.column.addEventListener('sortChanged', this.onSortChangedListener);
        this.onSortChanged();
    } else {
        this.eGui.removeChild(this.eSortDownButton);
        this.eGui.removeChild(this.eSortUpButton);
        this.eGui.removeChild(this.eSortRemoveButton);
    }
};
CustomHeader.prototype.onHeaderCheckBoxChanged = function () {
    if (this.eHeaderCheckBox.checked == true) {
        gridOptions.api.selectAll();
    } else {
        gridOptions.api.deselectAll();
    }
};
CustomHeader.prototype.onSortChanged = function () {
    function deactivate(toDeactivateItems) {
        toDeactivateItems.forEach(function (toDeactivate) {
            toDeactivate.className = toDeactivate.className.split(' ')[0]
        });
    }

    function activate(toActivate) {
        toActivate.className = toActivate.className + " active";
    }

    if (this.agParams.column.isSortAscending()) {
        deactivate([this.eSortUpButton, this.eSortRemoveButton]);
        activate(this.eSortDownButton)
    } else if (this.agParams.column.isSortDescending()) {
        deactivate([this.eSortDownButton, this.eSortRemoveButton]);
        activate(this.eSortUpButton)
    } else {
        deactivate([this.eSortUpButton, this.eSortDownButton]);
        activate(this.eSortRemoveButton)
    }
};
CustomHeader.prototype.getGui = function () {
    return this.eGui;
};
CustomHeader.prototype.onMenuClick = function () {
    this.agParams.showColumnMenu(this.eMenuButton);
};
CustomHeader.prototype.onSortRequested = function (order, event) {
    this.agParams.setSort(order, event.shiftKey);
};
CustomHeader.prototype.destroy = function () {
    if (this.onMenuClickListener) {
        this.eMenuButton.removeEventListener('click', this.onMenuClickListener)
    }
    this.eSortDownButton.removeEventListener('click', this.onSortRequestedListener);
    this.eSortUpButton.removeEventListener('click', this.onSortRequestedListener);
    this.eSortRemoveButton.removeEventListener('click', this.onSortRequestedListener);
    this.agParams.column.removeEventListener('sortChanged', this.onSortChangedListener);
    this.agParams.column.removeEventListener('change', this.onHeaderCheckBoxChangedListener);
};