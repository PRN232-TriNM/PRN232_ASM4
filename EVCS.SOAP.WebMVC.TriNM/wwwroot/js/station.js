let searchTimeout;
let currentPage = 1;
let currentPageSize = 10;
let connection;

document.addEventListener('DOMContentLoaded', function () {
    initializeSignalR();
    setupSearch();
    setupPagination();
});

function initializeSignalR() {
    const hubUrl = typeof signalRHubUrl !== 'undefined' ? signalRHubUrl : 'http://localhost:5046/stationHub';
    
    connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();

    connection.start()
        .then(() => {
            console.log('SignalR Connected');
            return connection.invoke('JoinStationGroup');
        })
        .then(() => {
            console.log('Joined Station Group');
        })
        .catch(err => {
            console.error('SignalR Connection Error:', err);
        });

    connection.on('StationCreated', function (station) {
        console.log('Station Created:', station);
        showNotification('Station created: ' + station.stationName, 'success');
        loadStations();
    });

    connection.on('StationUpdated', function (station) {
        console.log('Station Updated:', station);
        showNotification('Station updated: ' + station.stationName, 'info');
        updateStationRow(station);
    });

    connection.on('StationDeleted', function (stationId) {
        console.log('SignalR: StationDeleted event received for station ID:', stationId);
        showNotification('Station deleted (ID: ' + stationId + ')', 'warning');
        removeStationRow(stationId);
    });
}

function setupSearch() {
    const nameInput = document.getElementById('searchName');
    const locationInput = document.getElementById('searchLocation');
    const isActiveSelect = document.getElementById('searchIsActive');
    const pageSizeSelect = document.getElementById('pageSize');
    const clearBtn = document.getElementById('clearSearch');

    if (nameInput) {
        nameInput.addEventListener('input', function () {
            debounceSearch(500);
        });
    }

    if (locationInput) {
        locationInput.addEventListener('input', function () {
            debounceSearch(500);
        });
    }

    if (isActiveSelect) {
        isActiveSelect.addEventListener('change', function () {
            debounceSearch(500);
        });
    }

    if (pageSizeSelect) {
        pageSizeSelect.addEventListener('change', function () {
            currentPageSize = parseInt(this.value);
            currentPage = 1;
            performSearch();
        });
    }

    if (clearBtn) {
        clearBtn.addEventListener('click', function () {
            document.getElementById('searchName').value = '';
            document.getElementById('searchLocation').value = '';
            document.getElementById('searchIsActive').value = '';
            currentPage = 1;
            performSearch();
        });
    }
}

function debounceSearch(delay) {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(function () {
        currentPage = 1;
        performSearch();
    }, delay);
}

function performSearch() {
    const name = document.getElementById('searchName')?.value || '';
    const location = document.getElementById('searchLocation')?.value || '';
    const isActiveValue = document.getElementById('searchIsActive')?.value;
    const isActive = isActiveValue === '' ? null : isActiveValue === 'true';

    showLoading();

    const url = `/Station/Search?name=${encodeURIComponent(name)}&location=${encodeURIComponent(location)}&isActive=${isActive !== null ? isActive : ''}&pageNumber=${currentPage}&pageSize=${currentPageSize}`;

    fetch(url)
        .then(response => response.json())
        .then(data => {
            if (data.error) {
                showError(data.error);
            } else {
                updateTable(data.items);
                updatePagination(data);
            }
        })
        .catch(error => {
            console.error('Search error:', error);
            showError('Error performing search');
        })
        .finally(() => {
            hideLoading();
        });
}

function setupPagination() {
    document.querySelectorAll('.page-link').forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const page = parseInt(this.getAttribute('data-page'));
            if (page && page !== currentPage) {
                currentPage = page;
                performSearch();
            }
        });
    });
}

function updateTable(stations) {
    const tbody = document.getElementById('stationsTableBody');
    if (!tbody) return;

    tbody.innerHTML = '';

    if (stations.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="text-center">No stations found</td></tr>';
        return;
    }

    stations.forEach(station => {
        const row = document.createElement('tr');
        row.setAttribute('data-station-id', station.stationId);
        row.innerHTML = `
            <td>${escapeHtml(station.stationCode || '')}</td>
            <td>${escapeHtml(station.stationName || '')}</td>
            <td>${escapeHtml(station.address || '')}</td>
            <td>${escapeHtml(station.city || '')}</td>
            <td>${station.capacity || 0}</td>
            <td>${station.currentAvailable || 0}</td>
            <td>
                ${station.isActive 
                    ? '<span class="badge bg-success">Active</span>' 
                    : '<span class="badge bg-secondary">Inactive</span>'}
            </td>
            <td>
                <a href="/Station/Details/${station.stationId}" class="btn btn-sm btn-info">Details</a>
                <a href="/Station/Edit/${station.stationId}" class="btn btn-sm btn-warning">Edit</a>
                <a href="/Station/Delete/${station.stationId}" class="btn btn-sm btn-danger">Delete</a>
            </td>
        `;
        tbody.appendChild(row);
    });
}

function updatePagination(data) {
    const pagination = document.querySelector('.pagination');
    if (!pagination) return;

    pagination.innerHTML = '';

    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${!data.hasPreviousPage ? 'disabled' : ''}`;
    prevLi.innerHTML = `<a class="page-link" href="#" data-page="${data.pageNumber - 1}">Previous</a>`;
    if (!data.hasPreviousPage) {
        prevLi.querySelector('a').setAttribute('tabindex', '-1');
        prevLi.querySelector('a').setAttribute('aria-disabled', 'true');
    }
    pagination.appendChild(prevLi);

    const startPage = Math.max(1, data.pageNumber - 2);
    const endPage = Math.min(data.totalPages, data.pageNumber + 2);

    for (let i = startPage; i <= endPage; i++) {
        const li = document.createElement('li');
        li.className = `page-item ${i === data.pageNumber ? 'active' : ''}`;
        li.innerHTML = `<a class="page-link" href="#" data-page="${i}">${i}</a>`;
        pagination.appendChild(li);
    }

    const nextLi = document.createElement('li');
    nextLi.className = `page-item ${!data.hasNextPage ? 'disabled' : ''}`;
    nextLi.innerHTML = `<a class="page-link" href="#" data-page="${data.pageNumber + 1}">Next</a>`;
    if (!data.hasNextPage) {
        nextLi.querySelector('a').setAttribute('tabindex', '-1');
        nextLi.querySelector('a').setAttribute('aria-disabled', 'true');
    }
    pagination.appendChild(nextLi);

    const pageLinks = pagination.querySelectorAll('.page-link');
    pageLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const page = parseInt(this.getAttribute('data-page'));
            if (page && page !== currentPage) {
                currentPage = page;
                performSearch();
            }
        });
    });

    const infoDiv = document.querySelector('.text-center.text-muted');
    if (infoDiv) {
        const start = (data.pageNumber - 1) * data.pageSize + 1;
        const end = Math.min(data.pageNumber * data.pageSize, data.totalCount);
        infoDiv.textContent = `Hiển thị ${start} - ${end} trong tổng số ${data.totalCount} trạm`;
    }
}

function updateStationRow(station) {
    const row = document.querySelector(`tr[data-station-id="${station.stationId}"]`);
    if (row) {
        row.querySelector('td:nth-child(1)').textContent = station.stationCode || '';
        row.querySelector('td:nth-child(2)').textContent = station.stationName || '';
        row.querySelector('td:nth-child(3)').textContent = station.address || '';
        row.querySelector('td:nth-child(4)').textContent = station.city || '';
        row.querySelector('td:nth-child(5)').textContent = station.capacity || 0;
        row.querySelector('td:nth-child(6)').textContent = station.currentAvailable || 0;
        const statusCell = row.querySelector('td:nth-child(7)');
        statusCell.innerHTML = station.isActive 
            ? '<span class="badge bg-success">Active</span>' 
            : '<span class="badge bg-secondary">Inactive</span>';
    } else {
        loadStations();
    }
}

function removeStationRow(stationId) {
    console.log('Attempting to remove station row with ID:', stationId);
    const row = document.querySelector(`tr[data-station-id="${stationId}"]`);
    if (row) {
        console.log('Station row found, removing...');
        row.remove();
        showNotification('Station removed from list', 'info');
    } else {
        console.log('Station row not found, reloading stations...');
        loadStations();
    }
}

function loadStations() {
    performSearch();
}

function showLoading() {
    const indicator = document.getElementById('loadingIndicator');
    const table = document.getElementById('stationsTable');
    if (indicator) indicator.style.display = 'block';
    if (table) table.style.opacity = '0.5';
}

function hideLoading() {
    const indicator = document.getElementById('loadingIndicator');
    const table = document.getElementById('stationsTable');
    if (indicator) indicator.style.display = 'none';
    if (table) table.style.opacity = '1';
}

function showError(message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-danger alert-dismissible fade show';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.querySelector('.container main').prepend(alertDiv);
}

function showNotification(message, type) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type === 'success' ? 'success' : type === 'warning' ? 'warning' : 'info'} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.querySelector('.container main').prepend(alertDiv);
    
    setTimeout(() => {
        alertDiv.remove();
    }, 3000);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

