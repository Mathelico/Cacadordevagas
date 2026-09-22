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
  visualizada: boolean;
}

type Filtro = "TODAS" | "CANDIDATAR" | "ANALISAR" | "IGNORAR";

function App() {
  const [resumo, setResumo] = useState<ResumoVagas | null>(null);
  const [vagas, setVagas] = useState<Vaga[]>([]);
  const [vagaSelecionada, setVagaSelecionada] = useState<number | null>(null);
  const [filtro, setFiltro] = useState<Filtro>("TODAS");
  const [busca, setBusca] = useState("");
  const [limiteBusca, setLimiteBusca] = useState(1);
  const [buscandoVagas, setBuscandoVagas] = useState(false);
  const [mensagem, setMensagem] = useState("");

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

  useEffect(() => {
    carregarDados();
  }, []);

  async function buscarNovasVagas() {
    try {
      setBuscandoVagas(true);
      setMensagem("");

      const resposta = await fetch(
        `http://localhost:5245/api/vagas/analisar-e-salvar?limite=${limiteBusca}`,
        {
          method: "POST",
        }
      );

      if (!resposta.ok) {
        throw new Error("Erro ao buscar novas vagas.");
      }

      const dados = await resposta.json();

      setMensagem(
        `${dados.vagasSalvas} nova(s) vaga(s) analisada(s).`
      );

      await carregarDados();
    } catch (erro) {
      console.error(erro);
      setMensagem("Não foi possível buscar novas vagas.");
    } finally {
      setBuscandoVagas(false);
    }
  }
async function atualizarVisualizada(
  vaga: Vaga,
  visualizada: boolean
) {
  const resposta = await fetch(
    `http://localhost:5245/api/vagas/${vaga.id}/visualizada`,
    {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(visualizada),
    }
  );

  if (!resposta.ok) {
    return;
  }

  const vagaAtualizada = await resposta.json();

  setVagas((vagasAtuais) =>
    vagasAtuais.map((item) =>
      item.id === vaga.id ? vagaAtualizada : item
    )
  );
}
  if (!resumo) {
    return <h1>Carregando...</h1>;
  }

  const vagasFiltradas = (
    filtro === "TODAS"
      ? vagas
      : vagas.filter((vaga) => vaga.recomendacao === filtro)
  )
    .filter((vaga) => {
      const termo = busca.toLowerCase();

      return (
        vaga.cargo.toLowerCase().includes(termo) ||
        vaga.empresa.toLowerCase().includes(termo)
      );
    })
    .sort((a, b) => b.compatibilidade - a.compatibilidade);

  return (
    <div className="container">
      <h1>JobHunter AI</h1>

      <div className="cards">
        <div
          className={`card ${filtro === "TODAS" ? "card-ativo" : ""}`}
          onClick={() => {
            setFiltro("TODAS");
            setVagaSelecionada(null);
          }}
        >
          <h2>{resumo.total}</h2>
          <p>Vagas analisadas</p>
        </div>

        <div
          className={`card ${
            filtro === "CANDIDATAR" ? "card-ativo" : ""
          }`}
          onClick={() => {
            setFiltro("CANDIDATAR");
            setVagaSelecionada(null);
          }}
        >
          <h2>{resumo.candidatar}</h2>
          <p>Candidatar</p>
        </div>

        <div
          className={`card ${
            filtro === "ANALISAR" ? "card-ativo" : ""
          }`}
          onClick={() => {
            setFiltro("ANALISAR");
            setVagaSelecionada(null);
          }}
        >
          <h2>{resumo.analisar}</h2>
          <p>Analisar</p>
        </div>

        <div
          className={`card ${
            filtro === "IGNORAR" ? "card-ativo" : ""
          }`}
          onClick={() => {
            setFiltro("IGNORAR");
            setVagaSelecionada(null);
          }}
        >
          <h2>{resumo.ignorar}</h2>
          <p>Ignorar</p>
        </div>

        <div className="card">
          <h2>{resumo.mediaCompatibilidade}%</h2>
          <p>Média de compatibilidade</p>
        </div>
      </div>

      <div className="acoes">
        <select
          value={limiteBusca}
          onChange={(event) =>
            setLimiteBusca(Number(event.target.value))
          }
          disabled={buscandoVagas}
        >
          <option value={1}>1 vaga</option>
          <option value={3}>3 vagas</option>
          <option value={5}>5 vagas</option>
        </select>
        <button
          onClick={buscarNovasVagas}
          disabled={buscandoVagas}
        >
          {buscandoVagas
            ? "Buscando e analisando..."
            : "Buscar novas vagas"}
        </button>

        {mensagem && <p>{mensagem}</p>}
      </div>

      <div className="busca-container">
        <input
          type="text"
          placeholder="Buscar por cargo ou empresa..."
          value={busca}
          onChange={(event) => setBusca(event.target.value)}
        />
      </div>

      <h2 className="titulo-vagas">
        {filtro === "TODAS"
          ? "Vagas encontradas"
          : `Vagas: ${filtro}`}
      </h2>

      <div className="lista-vagas">
        {vagasFiltradas.map((vaga) => (
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

                <span
                  className={`status ${vaga.recomendacao.toLowerCase()}`}
                >
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
                
                <button
                  className={`botao-visualizada ${
                    vaga.visualizada ? "visualizada" : ""
                  }`}
                  onClick={(event) => {
                    event.stopPropagation();

                    atualizarVisualizada(
                      vaga,
                      !vaga.visualizada
                    );
                  }}
                >
                  {vaga.visualizada
                    ? "✓ Vaga visualizada"
                    : "Marcar como visualizada"}
                </button>

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