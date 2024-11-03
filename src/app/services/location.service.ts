import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LocationService {
  constructor() {}

  getLastLocation(location: any): string {
    if (!location) return '';
    return `${location.name}`;
  }

  getFullLocation(location: any): string {
    let locationNames = [];
    while (location) {
      locationNames.push(location.name);
      location = location.parent;
    }
    return locationNames.reverse().join(' > ');
  }
}
