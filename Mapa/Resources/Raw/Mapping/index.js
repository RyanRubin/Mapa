class App {
    initMap() {
        const bingMapsSource = new ol.source.BingMaps({
            key: 'AkGBls_Q10K9PW0jYdc1KuSPiF5qOXXICG3D9F2cT5QWLzdJsVYwgl8JYpnZg8sE',
            imagerySet: 'Aerial',
            tileLoadFunction: (imageTile, src) => {
                imageTile.getImage().src = src;
            }
        });

        const offlineMapsSource = new ol.source.OSM({
            tileLoadFunction: (imageTile, src) => {
                imageTile.getImage().src = src;
            }
        });

        new ol.Map({
            target: 'map',
            layers: [new ol.layer.Tile({
                source: bingMapsSource
            })],
            view: new ol.View({
                center: ol.proj.fromLonLat([-81.49559810202766, 41.469741498180625]),
                zoom: 18
            })
        });
    }
}

const app = new App();
