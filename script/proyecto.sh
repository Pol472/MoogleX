#!/bin/bash

#Comando para ejecutar el proyecto Moogle
run() {
  if [[ "$OSTYPE" == "linux-gnu" || "$OSTYPE" == "darwin"* ]]; 
  then
  make dev
else
  dotnet watch run --project MoogleServer
fi
}

#Comando para compilar el informe en pdf
report() {
  cd Informe
  if [ -z "$1" ]
  then
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape informe.tex </dev/null
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape informe.tex </dev/null
  else
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape informe.tex </dev/null
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape informe.tex </dev/null
  fi   
  cd ..
}

#Comando para compilar la presentacion en pdf
slides() {
  cd Presentacion
  if [ -z "$1" ]
  then
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentacion.tex </dev/null
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentacion.tex </dev/null
  else
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentacion.tex </dev/null
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentacion.tex </dev/null
  fi
  cd ..
}

#Comando para eliminar los archivos innecesarios producidos durante la compilacion
clean() {
  cd Informe
  rm -f informe.pdf informe.aux informe.fls informe.log informe.fdb_latexmk informe.out informe.synctex.gz informe.toc
  cd ..
  cd Presentacion
  rm -f presentacion.pdf presentacion.aux presentacion.fls presentacion.log presentacion.nav presentacion.synctex.gz presentacion.snm presentacion.toc presentacion.fdb_latexmk
  cd ..
  cd MoogleServer
  
  if [ -r  "bin" ]
  then
    rm -r bin obj
  fi
  
  cd ..
  cd MoogleEngine
  if [ -r  "bin" ]
  then
    rm -r bin obj
  fi
  cd ..
}

#Comando para mostrar el informe y compilarlo en caso de no haberlo hecho anteriormente
show_report() {
  if [ ! -f  "Informe/informe.pdf" ]
  then 
    report;
  fi

  if [ -z "$1" ]
  then
     if [[ "$OSTYPE" == "linux-gnu"* ]]; 
     then
     xdg-open Informe/informe.pdf
     elif [[ "$OSTYPE" == "darwin"* ]];
     then
     open Informe/informe.pdf
     else
      start Informe/informe.pdf
     fi
  else
    $1 Informe/informe.pdf
  fi
}

#Comando para mostrar la presentacion y compilarla en caso de no haberlo hecho anteriormente
show_slides() {
  if [ ! -f  "Presentacion/presentacion.pdf" ]
  then
    slides;
  fi
  
  if [ -z "$1" ]
  then

     if [[ "$OSTYPE" == "linux-gnu"* ]]; 
     then
      xdg-open Presentacion/presentacion.pdf
      elif [[ "$OSTYPE" == "darwin"* ]]; 
      then
      open Presentacion/presentacion.pdf
      else
      start  Presentacion/presentacion.pdf
    fi
    
  else
    $1 Presentacion/presentacion.pdf
  fi  
}

#Comando de informacion
info() {
  echo "El script consta de los siguientes comandos: "
  echo ""
  echo "run         -Para ejecutar el proyecto Moogle"
  echo "report      -Para compilar y generar el pdf del proyecto latex del informe"
  echo "slides      -Para compilar y generar el pdf del proyecto latex de la presentacion"
  echo "show_report -Para visualizar el informe y si el fichero pdf  no ha sido creado entonces generarlo"
  echo "show_slides -Para visualizar la presentación y si el fichero pdf no ha sido creado entonces generarlo"
  echo "clean       -Para eliminar todos los ficheros auxiliares que no forman parte del contenido del repositorio"
  echo "info        -Para visualizar esta información"
  echo ""
}

if [[ $# -eq 0 ]];
then
info;
exit 0
fi
cd ..
"$@"