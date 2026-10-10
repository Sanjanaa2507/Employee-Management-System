import { Component, AfterViewInit, inject } from '@angular/core';
import * as L from 'leaflet';
import 'leaflet-draw/dist/leaflet.draw.js';
import { LocationService } from '../../services/location.service';

@Component({
  selector: 'app-admin-map',
  templateUrl: './admin-map.component.html',
  styleUrls: ['./admin-map.component.scss']
})
export class AdminMapComponent implements AfterViewInit {

  private locationService = inject(LocationService);

  map!: L.Map;
  drawnItems = new L.FeatureGroup();
  employeeMarkers = L.layerGroup();

  ngAfterViewInit(): void {
    this.initMap();
    this.initDrawTools();
    this.loadEmployees();

    setTimeout(() => {
      this.map.invalidateSize();
    }, 0);
  }

  // ================= MAP INIT =================
  initMap() {
    this.map = L.map('map').setView([21.71, 72.99], 7);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap'
    }).addTo(this.map);

    this.map.addLayer(this.drawnItems);
    this.map.addLayer(this.employeeMarkers);
  }

  // ================= DRAW GEOFENCE =================
  initDrawTools() {
    const drawControl = new (L.Control as any).Draw({
      draw: {
        polygon: true,
        rectangle: true,
        circle: true,
        polyline: false,
        marker: false,
        circlemarker: false
      },
      edit: {
        featureGroup: this.drawnItems
      }
    });

    this.map.addControl(drawControl);

    this.map.on((L as any).Draw.Event.CREATED, (event: any) => {
      this.drawnItems.clearLayers();
      this.drawnItems.addLayer(event.layer);
    });
  }

  // ================= LOAD EMPLOYEES =================
  loadEmployees() {
    this.locationService.getAllEmployeeLocations().subscribe({
      next: (employees) => {
        this.employeeMarkers.clearLayers();

        employees.forEach(emp => {
          const icon = L.divIcon({
            className: '',
            html: `
              <div style="
                width:14px;
                height:14px;
                border-radius:50%;
                background:${emp.isInside ? 'green' : 'red'};
                border:2px solid white;">
              </div>
            `
          });

          L.marker([emp.latitude, emp.longitude], { icon })
            .bindPopup(`
              <b>${emp.name}</b><br/>
              ${emp.isInside ? 'Inside Geofence' : 'Outside Geofence'}<br/>
              ${new Date(emp.checkedAt).toLocaleString()}
            `)
            .addTo(this.employeeMarkers);
        });
      },
      error: err => {
        console.error('Failed to load employee locations', err);
      }
    });
  }
}
