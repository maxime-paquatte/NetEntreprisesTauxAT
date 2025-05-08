# Net-Entreprises - Taux AT

## Contexte

Les cabinets d'expertise comptable ont besoin de mettre à jour les taux AT de leurs clients.  
Le site net-entreprises.fr permet de récupérer les taux AT de chaque entreprise au format CRM, dans des fichiers XML.  
Ces fichiers contiennent le numéro SIREN de l'entreprise, mais pas sa raison sociale.

## Fonctionnement

Cette application Windows permet de sélectionner les fichiers XML (ou les fichiers ZIP contenant des fichiers XML) et de les traiter pour en extraire les informations suivantes :

- siren
- état
- type
- code_ctn
- section
- code_risque
- témoin_bureau
- taux
- date_effet
- date_calcul

Ces informations sont ensuite insérées dans un fichier Excel donné.  
Ce fichier peut déjà contenir des données (notamment la raison sociale et le SIREN) ; l'application mettra alors à jour les lignes existantes.

Par défaut, l'application cherche le SIREN dans la colonne `C` et insère les données à partir de la colonne `D`.

**Remarques :**

- Si plusieurs taux AT sont trouvés pour un même SIREN, l'application ajoutera les taux suivants à la fin du fichier avec la valeur `+++` en colonne `C`.
- Si le SIREN n'est pas trouvé dans le fichier Excel, l'application ajoutera une nouvelle ligne à la fin du fichier avec la valeur `---` en colonne `C`.
