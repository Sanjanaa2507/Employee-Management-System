import * as L from 'leaflet';

declare module 'leaflet' {
  namespace Control {
    class Draw extends Control {
      constructor(options?: any);
    }
  }
}
