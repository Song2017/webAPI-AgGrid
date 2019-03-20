var vkresult;
var varrequest;
var dataSource;
var agPagination = {
    PageIndex: 0,
    lastRow: null,
    PageSize: 10
};
// Columns
var columnDefs = [
    {
        headerName: "", width: 40, field: "selectcol", hide: false,
        lockPosition: true, lockPinned: true,
        lockVisible: true, pinned: "left", resizable: false, sortable: false, filter: false,
        suppressMenu: true, suppressMovable: true, suppressNavigable: true,
        checkboxSelection: function (params) { return true; }
    },
    {
        headerName: "UNIQUEKEY", field: "uniquekey", filter: 'agTextColumnFilter',
        filterParams: {defaultOption: 'startsWith'}
    },
    {
        headerName: "TAGNUMBER11", field: "tagnumber", sortable: true,
        enableRowGroup: true, filter: 'agTextColumnFilter',
        floatingFilterComponentParams: {debounceMs: 2000}
    },
    {
        headerName: "Valve Size", field: "valvesize", filter: 'agNumberColumnFilter'
    },
    { headerName: "Date Tested", field: "datetested", filter: 'agDateColumnFilter' }
];

// groupColumn
var groupColumn = {
    headerName: "Group Column",
    width: 200, 
    headerCheckboxSelectionFilteredOnly: true,
    cellRenderer: 'agGroupCellRenderer'
};

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
        menuTabs: ['filterMenuTab', 'generalMenuTab']
    },
    floatingFilter: true,
    autoGroupColumnDef: groupColumn,
    columnDefs: columnDefs,
    multiSortKey: 'ctrl',
    debug: false,

    rowModelType: 'infinite',
    rowDragManaged: true,
    rowSelection: 'multiple',
    rowDeselection: true,
    rowGroupPanelShow: 'always',

    //cacheBlockSize: min(pagesize), in case no data to show
    cacheBlockSize: 10,//Lazy-loading: with the grid options property: cacheBlockSize = 100 data will be fetched in blocks of 100 rows at a time.
    maxBlocksInCache: 1000,//to limit the amount of data cached in the grid you can set maxBlocksInCache via the gridOptions.
    infiniteInitialRowCount: 5,//How many rows to initially allow the user to scroll to. 
    animateRows: true,
    pagination: true,
    paginationPageSize: 10,
    onPaginationChanged: onPaginationChanged,
    //quickFilterText: null,

    sideBar: {
        toolPanels: [{
            id: 'columns',
            labelDefault: 'Columns',
            labelKey: 'columns',
            iconKey: 'columns',
            toolPanel: 'agColumnsToolPanel',
            toolPanelParams: {
                suppressPivots: true,
                suppressPivotMode: true,
                suppressValues: true
            }
        }]
    },

    components: {
        booleanCellRenderer: booleanCellRenderer
    },
};

// setup the grid after the page has finished loading
document.addEventListener('DOMContentLoaded', function () {
    var gridDiv = document.querySelector('#myGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    fetch('https://localhost:44364/api/aggrid/GetDataColumns/uspGetCVList').then(function (response) {
        return response.json();
    }).then(function (data) {
        console.log(JSON.parse(data))
        gridOptions.api.setColumnDefs(columnDefs.concat(JSON.parse(data)));
    })

    gridOptions.api.setDatasource(EnterpriseDatasource);
});

// Datasource
var EnterpriseDatasource = {
    rowCount: 100,
    getRows: function (params) {
        var request = params;
        request['PageIndex'] = agPagination.PageIndex;
        request['PageSize'] = agPagination.PageSize;
        request['filterModel'] = formatFilterModel(request.filterModel);
        let jsonRequest = JSON.stringify(request, null, 2);
        let httpRequest = new XMLHttpRequest();
        varrequestjson = jsonRequest
        httpRequest.open('POST', 'https://localhost:44364/api/aggrid/GetAllData');
        httpRequest.setRequestHeader("Content-type", "application/json");
        httpRequest.send(jsonRequest);
        httpRequest.onreadystatechange = () => {
            if (httpRequest.readyState === 4 && httpRequest.status === 200) {
                let result = JSON.parse(httpRequest.responseText);
                vkresult = result;
                var newDataLength = result.data.length;
                if (newDataLength === 0 || newDataLength < agPagination.PageSize) {
                    agPagination.lastRow = agPagination.PageIndex * agPagination.PageSize + newDataLength;
                }
                params.successCallback(result.data, result.lastRow);
            }
        };
    }
}


// Functions
// SelectedRows
function getSelectedRows() {
    var selectedNodes = gridOptions.api.getSelectedNodes();
    var selecteddata = selectedNodes.map(function (node) {
        return node.data
    }).map(function (node) {
        return node.UNIQUEKEY + '_' + node.model + '_' + node.A_ACTION
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
        allColumnIds.push(column.colId);
    });
    gridOptions.columnApi.autoSizeColumns(allColumnIds.slice(1));
}
// Check box render
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
    let value = Number(document.getElementById('page-size').value);
    gridOptions.api.paginationSetPageSize(value);
    agPagination.PageSize = value;
    gridOptions.cacheBlockSize = value;
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
    let aryFilter = []
    let objChild = {}
    let aryCondition = []
    for (let filter in filterModels) {
        if (filterModels[filter].operator) {
            objChild["head"] = { "field": filter, "operate": filterModels[filter].operator };

            if (filterModels[filter].condition1)
                aryCondition.push(filterModels[filter].condition1);
            if (filterModels[filter].condition2)
                aryCondition.push(filterModels[filter].condition2);
        }
        else {
            objChild["head"] ={ "field": filter, "operate": "" };
            aryCondition.push(filterModels[filter]);
        }
        objChild["condition"] = aryCondition;
        aryFilter.push(objChild);
        aryCondition = [];
        objChild = {};
    }

    return aryFilter;
}