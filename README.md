# KartCaseStudy

# Ön Bilgi

Unity 2022.3.26f1 versiyonu ile yapıldı. Oynanmaya hazır 84MB android APK buildi Playable klasörüne eklendi.

# Ne İçeriyor?

Genel hatlarıyla sistem mimarisi yapıldı.\
Mimaride Dependency Injection ve Observer Pattern gibi tasarım desenleri kullanıldı.\
Mario Kart oyununun temel mekağini olan araç sürme ve drift yapma mekanikleri gerçeğine olabildiğince yakın bir şekilde gerçek fizik sistemi kullanılarak entegre edildi.\
Tamamen hareketle entegre çalışan animasyon sistemi eklendi.\
Gerçek motor sistemine yakın bir şekilde, hızlandıkça artan motor ses sistemi eklendi.\
Basit çevre tasarımı yapıldı.\
Yükleme ve sahne sistemi entegre edildi.\
Adaptif bir şekilde çalışan UI entegrasyonu yapıldı.\
Tur sistemi eklendi.\
Sıralama ve checkpoint sistemi eklendi.\
Geliştirmeye açık bir şekilde gerekli yerlere uygun sistemler hazırlandı.

# Gözükmeyen Kısım

Yarış sonu verileri kaydetme özelliği eklendi.\
Farklı karakterlerin eklenmesi için uygun altyapılar oluşturuldu.\
Olası bir multiplayer senaryosu için, yarış sistemi modüler bir yapıda oluşturuldu.\
Her bir yarışçı, kolay şekilde düzenlenebilir modifier classlarında ayarlanabilecek şekilde tasarlandı.\
Olası bir upgrade senaryosu için, araçların statları kolay bir şekilde değiştirilebilecek şekilde tasarlandı. Örneğin yoldan alınabilecek speed boostlar veya rakibe atılacak füzeler.

# Ne / Neden İçermiyor?

Case çalışmasının bir son ürün ortaya koymak yerine niyetin ve yeteneklerin etkili bir şekilde ortaya konup, kısa ve öz şekilde sunulması taraftarı olduğum için case çalışmasını tamamen bitirilmiş bir oyun halinde sunmamak ve kısa tutmak için:\
Yapay zeka sistemleri eklenmedi.\
Alınabilecek ve kullanılabilecek herhangi bir güçlendirme sistemi eklenmedi.

# Ne Eklenebilir?

Proje tamamen gelişmeye açık bir şekilde geliştirildi. Geliştirmeye başlanıldığı zaman kısa zamanda yapay zeka rakipleri ve sürüş yolundan alınabilecek çeşitli güçler çok kısa bir süre içerisinde eklenebilir. Universal Render Pipeline kullanıldığı için grafiksel anlamda hızlı değişiklikler yapılabilir. Farklı araçlar ve karakterlerin eklenmesine açık bir şekilde geliştirildi. Sahne sistemi entegre edildiği için, kolaylıkla yarıştan önce karaketer ve araç seçme ekranı dahil edilebilir.

# Test Aşaması

Geliştirilen prototip PC, Samsung A54 ve Samsung S24 cihazlarında test edildi. Test sonuçlarında herhangi bir olumsuzlukla karşılanmadı. 60 FPS olmak üzere akıcı bir şekilde oynandı. Düşük donanıma sahip herhangi bir cihazda test edilemediği için, oyun akışını etkileyecek her türlü olumsuz etken için gerekli optimizasyonlar eklenecektir.
