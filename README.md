# KartCaseStudy

# Ön Bilgi

Unity 2022.3.26f1 versiyonu ile yapıldı.

# Ne İçeriyor?

Genel hatlarıyla sistem mimarisi yapıldı.
Mimaride Dependency Injection ve Observer Pattern gibi tasarım desenleri kullanıldı.
Mario Kart oyununun temel mekağini olan araç sürme ve drift yapma mekanikleri gerçeğine olabildiğince yakın bir şekilde gerçek fizik sistemi kullanılarak entegre edildi.
Tamamen hareketle entegre çalışan animasyon sistemi eklendi.
Basit çevre tasarımı yapıldı.
Yükleme ve sahne sistemi entegre edildi.
Adaptif bir şekilde çalışan UI entegrasyonu yapıldı.
Geliştirmeye açık bir şekilde gerekli yerlere uygun sistemler hazırlandı.

# Ne / Neden İçermiyor?

Case çalışmasının bir son ürün ortaya koymak yerine niyetin ve yeteneklerin etkili bir şekiklde ortaya konup, kısa ve öz şekilde sunulması taraftarı olduğum için;
Case çalışmasını tamamen bitirilmiş bir oyun halinde sunmamak ve kısa tutmak için yapay zeka sistemleri eklenmedi.
Herhangi bir yapay zeka olmadığı için bir yarış sistemi entegre edilmedi. Yarış sıralaması ve yarışın başlangıç ve bitiş sistemleri eklenmedi.

# Ne Eklenebilir?

Proje tamamen gelişmeye açık bir şekilde geliştirildi. Geliştirmeye başlanıldığı zaman kısa zamanda yapay zeka rakipleri, sürüş yolundan alınabilecek çeşitli güçler, sıralama ve checkpoint sistemi çok kısa bir süre içerisinde eklenebilir. Universal Render Pipeline kullanıldığı için grafiksel anlamda hızlı değişiklikler yapılabilir. Farklı araçlar ve karakterlerin eklenmesine açık bir şekilde geliştirildi.

# Test Aşaması

Geliştirilen prototip PC, Samsung A54 ve Samsung S24 cihazlarında test edildi. Test sonuçlarında herhangi bir olumsuzlukla karşılanmadı. 60 FPS olmak üzere akıcı bir şekilde oynandı. Düşük donanıma sahip herhangi bir cihazda test edilemediği için, oyun akışını etkileyecek her türlü olumsuz etken için gerekli optimizasyonlar eklenecektir.
