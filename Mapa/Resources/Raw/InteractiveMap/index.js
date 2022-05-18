class App {
    map;
    tileLayer;
    bingMapsSource;
    offlineMapsSource;
    vectorLayer;
    vectorSource;
    view;

    initMap() {
        // window.addEventListener('contextmenu', e => e.preventDefault());

        this.bingMapsSource = new ol.source.BingMaps({
            key: 'AkGBls_Q10K9PW0jYdc1KuSPiF5qOXXICG3D9F2cT5QWLzdJsVYwgl8JYpnZg8sE',
            imagerySet: 'Aerial',
            maxZoom: 19, // https://openlayers.org/en/latest/examples/bing-maps.html
            tileLoadFunction: (imageTile, src) => {
                imageTile.getImage().src = src;
                // console.log(imageTile.tileCoord);
            }
        });

        this.offlineMapsSource = new ol.source.OSM({
            tileLoadFunction: (imageTile, src) => {
                imageTile.getImage().src = src;
            }
        });

        this.tileLayer = new ol.layer.Tile({
            source: this.bingMapsSource
        });

        this.vectorSource = new ol.source.Vector();

        this.vectorLayer = new ol.layer.Vector({
            source: this.vectorSource
        });

        this.view = new ol.View();

        this.map = new ol.Map({
            target: 'map',
            layers: [this.tileLayer, this.vectorLayer],
            view: this.view
        });

        const popup = new ol.Overlay({
            element: document.getElementById('popup')
        });
        this.map.addOverlay(popup);

        let popover;
        this.map.on('click', (e) => {
            const popupEl = popup.getElement();
            const coordinate = e.coordinate;
            const hdms = ol.coordinate.toStringHDMS(ol.proj.toLonLat(coordinate));

            if (popover) {
                popover.dispose();
            }
            popup.setPosition(coordinate);
            popover = new bootstrap.Popover(popupEl, {
                container: popupEl,
                placement: 'top',
                animation: false,
                html: true,
                content: `<p>The location you clicked was:</p><code>${hdms}</code>`
            });
            popover.show();
        });

        const blastCoord = ol.proj.fromLonLat([-81.49559810202766, 41.469741498180625]);
        const npsCoord = ol.proj.fromLonLat([-81.495, 41.469]);
        const seismographCoord = ol.proj.fromLonLat([-81.496, 41.470]);

        this.addCircle(blastCoord, 'blue');
        this.addIcon(blastCoord, 'img/icon.png');

        this.addCircle(npsCoord, 'yellow');
        this.addLine(blastCoord, npsCoord, 'yellow');

        this.addCircle(seismographCoord, 'red');
        this.addLine(blastCoord, seismographCoord, 'red');

        this.fitToFeaturesExtent();
    }

    addIcon(coord, src) {
        const iconFeature = new ol.Feature({
            geometry: new ol.geom.Point(coord)
        });
        iconFeature.setStyle(new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 46],
                anchorXUnits: 'fraction',
                anchorYUnits: 'pixels',
                src: src
            })
        }));

        this.vectorSource.addFeature(iconFeature);
    }

    addCircle(coord, color) {
        const circleFeature = new ol.Feature({
            geometry: new ol.geom.Point(coord)
        });
        circleFeature.setStyle(new ol.style.Style({
            image: new ol.style.Circle({
                radius: 8,
                fill: new ol.style.Fill({
                    color: color
                })
            })
        }));

        this.vectorSource.addFeature(circleFeature);
    }

    addLine(coord1, coord2, color) {
        const lineFeature = new ol.Feature({
            geometry: new ol.geom.LineString([coord1, coord2])
        });
        lineFeature.setStyle(new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: color,
                width: 1.25,
                lineDash: [5, 5]
            })
        }));

        this.vectorSource.addFeature(lineFeature);
    }

    fitToFeaturesExtent() {
        const vectorExtent = this.vectorSource.getExtent();
        this.view.fit(vectorExtent, {
            padding: [25, 25, 25, 25],
            maxZoom: 19
        });
    }
}

const app = new App();
