import { useEffect, useState } from "react";
import "./App.css";

interface ResumoVagas {
  total: number;
  candidatar: number;
  analisar: number;
  ignorar: number;
  mediaCompatibilidade: number;
}

interface Vaga {
  id: number;
  cargo: string;
  empresa: string;
  descricao: string;
  link: string;
  compatibilidade: number;
  recomendacao: string;
  nivelVaga: string;
  justificativa: string;
}

function App() {
  const [resumo, setResumo] = useState<ResumoVagas | null>(null);
  const [vagas, setVagas] = useState<Vaga[]>([]);
  const [vagaSelecionada, setVagaSelecionada] = useState<number | null>(null);

  useEffect(() => {
    async function carregarDados() {
      const respostaResumo = await fetch(
        "http://localhost:5245/api/vagas/resumo"
      );

      const dadosResumo = await respostaResumo.json();

      setResumo(dadosResumo);

      const respostaVagas = await fetch(
        "http://localhost:5245/api/vagas"
      );

      const dadosVagas = await respostaVagas.json();

      setVagas(dadosVagas);
    }

    carregarDados();
  }, []);

  if (!resumo) {
    return <h1>Carregando...</h1>;
  }

  return (
    <div className="container">
      <h1>JobHunter AI</h1>

      <div className="cards">
        <div className="card">
          <h2>{resumo.total}</h2>
          <p>Vagas analisadas</p>
        </div>

        <div className="card">
          <h2>{resumo.candidatar}</h2>
          <p>Candidatar</p>
        </div>

        <div className="card">
          <h2>{resumo.analisar}</h2>
          <p>Analisar</p>
        </div>

        <div className="card">
          <h2>{resumo.ignorar}</h2>
          <p>Ignorar</p>
        </div>

        <div className="card">
          <h2>{resumo.mediaCompatibilidade}%</h2>
          <p>Média de compatibilidade</p>
        </div>
      </div>

      <h2 className="titulo-vagas">Vagas encontradas</h2>

      <div className="lista-vagas">
        {vagas.map((vaga) => (
          <div
            className="vaga-card"
            key={vaga.id}
            onClick={() =>
              setVagaSelecionada(
                vagaSelecionada === vaga.id ? null : vaga.id
              )
            }
          >
          <div className="vaga-topo">
            <div>
              <h3>{vaga.cargo}</h3>
              <p className="empresa">{vaga.empresa}</p>
            </div>

            <div className="vaga-info">
              <strong>{vaga.compatibilidade}%</strong>

              <span className={`status ${vaga.recomendacao.toLowerCase()}`}>
                {vaga.recomendacao}
              </span>
            </div>
          </div>

            {vagaSelecionada === vaga.id && (
              <div className="vaga-detalhes">
                <p>
                  <strong>Nível:</strong> {vaga.nivelVaga}
                </p>

                <p>
                  <strong>Justificativa da IA:</strong>
                </p>

                <p>{vaga.justificativa}</p>

                <p>
                  <strong>Descrição:</strong>
                </p>

                <p>{vaga.descricao}</p>

                <a
                  href={vaga.link}
                  target="_blank"
                  rel="noreferrer"
                  onClick={(event) => event.stopPropagation()}
                >
                  Abrir vaga
                </a>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

export default App;